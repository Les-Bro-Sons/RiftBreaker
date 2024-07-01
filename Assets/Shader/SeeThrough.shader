Shader "RiftBreaker/SeeThrough"
{
    Properties
    {
        _PlayerPosition("PlayerPosition", Vector) = (0.5, 0.5, 0, 0)
        _Size("Size", Float) = 1
        _Smoothness("Smoothness", Range(0, 1)) = 0.5
        _Opacity("Opacity", Range(0, 1)) = 1
        [HideInInspector]_QueueOffset("_QueueOffset", Float) = 0
        [HideInInspector]_QueueControl("_QueueControl", Float) = -1
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "UniversalMaterialType" = "Unlit"
            "Queue"="Transparent"
            "DisableBatching"="False"
            "ShaderGraphShader"="true"
            "ShaderGraphTargetId"="UniversalUnlitSubTarget"
        }
        Pass
        {
            Name "Universal Forward"
            Tags
            {
                // LightMode: <None>
            }
        
        // Render State
        Cull Back
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite On
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma multi_compile_instancing
        #pragma multi_compile_fog
        #pragma instancing_options renderinglayer
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        #pragma multi_compile _ LIGHTMAP_ON
        #pragma multi_compile _ DIRLIGHTMAP_COMBINED
        #pragma shader_feature _ _SAMPLE_GI
        #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
        #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_UNLIT
        #define _FOG_FRAGMENT 1
        #define _SURFACE_TYPE_TRANSPARENT 1
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float2 NDCPosition;
             float2 PixelPosition;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS : INTERP0;
             float3 normalWS : INTERP1;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.positionWS.xyz = input.positionWS;
            output.normalWS.xyz = input.normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.positionWS.xyz;
            output.normalWS = input.normalWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float2 _PlayerPosition;
        float _Size;
        float _Smoothness;
        float _Opacity;
        CBUFFER_END
        
        
        // Object and Global properties
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Remap_float2(float2 In, float2 InMinMax, float2 OutMinMax, out float2 Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_Subtract_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A - B;
        }
        
        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A / B;
        }
        
        void Unity_Length_float2(float2 In, out float Out)
        {
            Out = length(In);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }
        
        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Property_73b5f521c9a1434b94e68c9424ac3949_Out_0_Float = _Smoothness;
            float4 _ScreenPosition_acce4db933974d1996b33954074332c6_Out_0_Vector4 = float4(IN.NDCPosition.xy, 0, 0);
            float2 _Property_d2fb2bd91c8c49aa95c9b94062e6f1a0_Out_0_Vector2 = _PlayerPosition;
            float2 _Remap_ac5142f5a7444be489aeb5076ae155bf_Out_3_Vector2;
            Unity_Remap_float2(_Property_d2fb2bd91c8c49aa95c9b94062e6f1a0_Out_0_Vector2, float2 (0, 1), float2 (0.5, -1.5), _Remap_ac5142f5a7444be489aeb5076ae155bf_Out_3_Vector2);
            float2 _Add_48dac6c54ec542348fe17b42048bfe19_Out_2_Vector2;
            Unity_Add_float2((_ScreenPosition_acce4db933974d1996b33954074332c6_Out_0_Vector4.xy), _Remap_ac5142f5a7444be489aeb5076ae155bf_Out_3_Vector2, _Add_48dac6c54ec542348fe17b42048bfe19_Out_2_Vector2);
            float2 _TilingAndOffset_9589f15f948e443db747b44141f4aa02_Out_3_Vector2;
            Unity_TilingAndOffset_float((_ScreenPosition_acce4db933974d1996b33954074332c6_Out_0_Vector4.xy), float2 (1, 1), _Add_48dac6c54ec542348fe17b42048bfe19_Out_2_Vector2, _TilingAndOffset_9589f15f948e443db747b44141f4aa02_Out_3_Vector2);
            float2 _Multiply_c5784441aa3d4c399fa12fc8ce686df9_Out_2_Vector2;
            Unity_Multiply_float2_float2(_TilingAndOffset_9589f15f948e443db747b44141f4aa02_Out_3_Vector2, float2(2, 2), _Multiply_c5784441aa3d4c399fa12fc8ce686df9_Out_2_Vector2);
            float2 _Subtract_7d595324968a4210ab4a41d20f8c0acd_Out_2_Vector2;
            Unity_Subtract_float2(_Multiply_c5784441aa3d4c399fa12fc8ce686df9_Out_2_Vector2, float2(1, 1), _Subtract_7d595324968a4210ab4a41d20f8c0acd_Out_2_Vector2);
            float _Divide_f9fda8aa789847349ef190dff711f8e2_Out_2_Float;
            Unity_Divide_float(unity_OrthoParams.y, unity_OrthoParams.x, _Divide_f9fda8aa789847349ef190dff711f8e2_Out_2_Float);
            float _Property_b22295ca229a4619a90f4628fcffdc80_Out_0_Float = _Size;
            float _Multiply_2c54520d2f6243389b8b126337cd5264_Out_2_Float;
            Unity_Multiply_float_float(_Divide_f9fda8aa789847349ef190dff711f8e2_Out_2_Float, _Property_b22295ca229a4619a90f4628fcffdc80_Out_0_Float, _Multiply_2c54520d2f6243389b8b126337cd5264_Out_2_Float);
            float2 _Vector2_e090312b2c9a40c3977d9897847677e5_Out_0_Vector2 = float2(_Multiply_2c54520d2f6243389b8b126337cd5264_Out_2_Float, _Property_b22295ca229a4619a90f4628fcffdc80_Out_0_Float);
            float2 _Divide_9bfc6c9c2f0d47a4b3c5a7f3c0543bbd_Out_2_Vector2;
            Unity_Divide_float2(_Subtract_7d595324968a4210ab4a41d20f8c0acd_Out_2_Vector2, _Vector2_e090312b2c9a40c3977d9897847677e5_Out_0_Vector2, _Divide_9bfc6c9c2f0d47a4b3c5a7f3c0543bbd_Out_2_Vector2);
            float _Length_0ab2951ca39b4bb4849df4df429e6289_Out_1_Float;
            Unity_Length_float2(_Divide_9bfc6c9c2f0d47a4b3c5a7f3c0543bbd_Out_2_Vector2, _Length_0ab2951ca39b4bb4849df4df429e6289_Out_1_Float);
            float _OneMinus_7c9727e510c64c68b4a5b95359a257cc_Out_1_Float;
            Unity_OneMinus_float(_Length_0ab2951ca39b4bb4849df4df429e6289_Out_1_Float, _OneMinus_7c9727e510c64c68b4a5b95359a257cc_Out_1_Float);
            float _Saturate_0f4422edb5f74268aa6a263c205182ee_Out_1_Float;
            Unity_Saturate_float(_OneMinus_7c9727e510c64c68b4a5b95359a257cc_Out_1_Float, _Saturate_0f4422edb5f74268aa6a263c205182ee_Out_1_Float);
            float _Smoothstep_6bcd6f29d62a46efa8684f71124ef46e_Out_3_Float;
            Unity_Smoothstep_float(float(0), _Property_73b5f521c9a1434b94e68c9424ac3949_Out_0_Float, _Saturate_0f4422edb5f74268aa6a263c205182ee_Out_1_Float, _Smoothstep_6bcd6f29d62a46efa8684f71124ef46e_Out_3_Float);
            float _Property_8d1db5cc03694ac69a1992f804d0d44f_Out_0_Float = _Opacity;
            float _Multiply_e624193bbe004f1a83454d49be3168fb_Out_2_Float;
            Unity_Multiply_float_float(_Smoothstep_6bcd6f29d62a46efa8684f71124ef46e_Out_3_Float, _Property_8d1db5cc03694ac69a1992f804d0d44f_Out_0_Float, _Multiply_e624193bbe004f1a83454d49be3168fb_Out_2_Float);
            float _OneMinus_930a063f193c439bacd5f0fef9ba15de_Out_1_Float;
            Unity_OneMinus_float(_Multiply_e624193bbe004f1a83454d49be3168fb_Out_2_Float, _OneMinus_930a063f193c439bacd5f0fef9ba15de_Out_1_Float);
            surface.BaseColor = IsGammaSpace() ? float3(0.5, 0.5, 0.5) : SRGBToLinear(float3(0.5, 0.5, 0.5));
            surface.Alpha = _OneMinus_930a063f193c439bacd5f0fef9ba15de_Out_1_Float;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x < 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
            #else
            output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x > 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
            #endif
        
            output.NDCPosition = output.PixelPosition.xy / _ScaledScreenParams.xy;
            output.NDCPosition.y = 1.0f - output.NDCPosition.y;
        
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "DepthNormalsOnly"
            Tags
            {
                "LightMode" = "DepthNormalsOnly"
            }
        
        // Render State
        Cull Back
        ZTest LEqual
        ZWrite On
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma multi_compile_instancing
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        #pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define VARYINGS_NEED_NORMAL_WS
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHNORMALSONLY
        #define _SURFACE_TYPE_TRANSPARENT 1
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float2 NDCPosition;
             float2 PixelPosition;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 normalWS : INTERP0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.normalWS.xyz = input.normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.normalWS = input.normalWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float2 _PlayerPosition;
        float _Size;
        float _Smoothness;
        float _Opacity;
        CBUFFER_END
        
        
        // Object and Global properties
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Remap_float2(float2 In, float2 InMinMax, float2 OutMinMax, out float2 Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_Subtract_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A - B;
        }
        
        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A / B;
        }
        
        void Unity_Length_float2(float2 In, out float Out)
        {
            Out = length(In);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }
        
        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Property_73b5f521c9a1434b94e68c9424ac3949_Out_0_Float = _Smoothness;
            float4 _ScreenPosition_acce4db933974d1996b33954074332c6_Out_0_Vector4 = float4(IN.NDCPosition.xy, 0, 0);
            float2 _Property_d2fb2bd91c8c49aa95c9b94062e6f1a0_Out_0_Vector2 = _PlayerPosition;
            float2 _Remap_ac5142f5a7444be489aeb5076ae155bf_Out_3_Vector2;
            Unity_Remap_float2(_Property_d2fb2bd91c8c49aa95c9b94062e6f1a0_Out_0_Vector2, float2 (0, 1), float2 (0.5, -1.5), _Remap_ac5142f5a7444be489aeb5076ae155bf_Out_3_Vector2);
            float2 _Add_48dac6c54ec542348fe17b42048bfe19_Out_2_Vector2;
            Unity_Add_float2((_ScreenPosition_acce4db933974d1996b33954074332c6_Out_0_Vector4.xy), _Remap_ac5142f5a7444be489aeb5076ae155bf_Out_3_Vector2, _Add_48dac6c54ec542348fe17b42048bfe19_Out_2_Vector2);
            float2 _TilingAndOffset_9589f15f948e443db747b44141f4aa02_Out_3_Vector2;
            Unity_TilingAndOffset_float((_ScreenPosition_acce4db933974d1996b33954074332c6_Out_0_Vector4.xy), float2 (1, 1), _Add_48dac6c54ec542348fe17b42048bfe19_Out_2_Vector2, _TilingAndOffset_9589f15f948e443db747b44141f4aa02_Out_3_Vector2);
            float2 _Multiply_c5784441aa3d4c399fa12fc8ce686df9_Out_2_Vector2;
            Unity_Multiply_float2_float2(_TilingAndOffset_9589f15f948e443db747b44141f4aa02_Out_3_Vector2, float2(2, 2), _Multiply_c5784441aa3d4c399fa12fc8ce686df9_Out_2_Vector2);
            float2 _Subtract_7d595324968a4210ab4a41d20f8c0acd_Out_2_Vector2;
            Unity_Subtract_float2(_Multiply_c5784441aa3d4c399fa12fc8ce686df9_Out_2_Vector2, float2(1, 1), _Subtract_7d595324968a4210ab4a41d20f8c0acd_Out_2_Vector2);
            float _Divide_f9fda8aa789847349ef190dff711f8e2_Out_2_Float;
            Unity_Divide_float(unity_OrthoParams.y, unity_OrthoParams.x, _Divide_f9fda8aa789847349ef190dff711f8e2_Out_2_Float);
            float _Property_b22295ca229a4619a90f4628fcffdc80_Out_0_Float = _Size;
            float _Multiply_2c54520d2f6243389b8b126337cd5264_Out_2_Float;
            Unity_Multiply_float_float(_Divide_f9fda8aa789847349ef190dff711f8e2_Out_2_Float, _Property_b22295ca229a4619a90f4628fcffdc80_Out_0_Float, _Multiply_2c54520d2f6243389b8b126337cd5264_Out_2_Float);
            float2 _Vector2_e090312b2c9a40c3977d9897847677e5_Out_0_Vector2 = float2(_Multiply_2c54520d2f6243389b8b126337cd5264_Out_2_Float, _Property_b22295ca229a4619a90f4628fcffdc80_Out_0_Float);
            float2 _Divide_9bfc6c9c2f0d47a4b3c5a7f3c0543bbd_Out_2_Vector2;
            Unity_Divide_float2(_Subtract_7d595324968a4210ab4a41d20f8c0acd_Out_2_Vector2, _Vector2_e090312b2c9a40c3977d9897847677e5_Out_0_Vector2, _Divide_9bfc6c9c2f0d47a4b3c5a7f3c0543bbd_Out_2_Vector2);
            float _Length_0ab2951ca39b4bb4849df4df429e6289_Out_1_Float;
            Unity_Length_float2(_Divide_9bfc6c9c2f0d47a4b3c5a7f3c0543bbd_Out_2_Vector2, _Length_0ab2951ca39b4bb4849df4df429e6289_Out_1_Float);
            float _OneMinus_7c9727e510c64c68b4a5b95359a257cc_Out_1_Float;
            Unity_OneMinus_float(_Length_0ab2951ca39b4bb4849df4df429e6289_Out_1_Float, _OneMinus_7c9727e510c64c68b4a5b95359a257cc_Out_1_Float);
            float _Saturate_0f4422edb5f74268aa6a263c205182ee_Out_1_Float;
            Unity_Saturate_float(_OneMinus_7c9727e510c64c68b4a5b95359a257cc_Out_1_Float, _Saturate_0f4422edb5f74268aa6a263c205182ee_Out_1_Float);
            float _Smoothstep_6bcd6f29d62a46efa8684f71124ef46e_Out_3_Float;
            Unity_Smoothstep_float(float(0), _Property_73b5f521c9a1434b94e68c9424ac3949_Out_0_Float, _Saturate_0f4422edb5f74268aa6a263c205182ee_Out_1_Float, _Smoothstep_6bcd6f29d62a46efa8684f71124ef46e_Out_3_Float);
            float _Property_8d1db5cc03694ac69a1992f804d0d44f_Out_0_Float = _Opacity;
            float _Multiply_e624193bbe004f1a83454d49be3168fb_Out_2_Float;
            Unity_Multiply_float_float(_Smoothstep_6bcd6f29d62a46efa8684f71124ef46e_Out_3_Float, _Property_8d1db5cc03694ac69a1992f804d0d44f_Out_0_Float, _Multiply_e624193bbe004f1a83454d49be3168fb_Out_2_Float);
            float _OneMinus_930a063f193c439bacd5f0fef9ba15de_Out_1_Float;
            Unity_OneMinus_float(_Multiply_e624193bbe004f1a83454d49be3168fb_Out_2_Float, _OneMinus_930a063f193c439bacd5f0fef9ba15de_Out_1_Float);
            surface.Alpha = _OneMinus_930a063f193c439bacd5f0fef9ba15de_Out_1_Float;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x < 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
            #else
            output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x > 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
            #endif
        
            output.NDCPosition = output.PixelPosition.xy / _ScaledScreenParams.xy;
            output.NDCPosition.y = 1.0f - output.NDCPosition.y;
        
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthNormalsOnlyPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "ShadowCaster"
            Tags
            {
                "LightMode" = "ShadowCaster"
            }
        
        // Render State
        Cull Back
        ZTest LEqual
        ZWrite On
        ColorMask 0
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma multi_compile_instancing
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define VARYINGS_NEED_NORMAL_WS
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_SHADOWCASTER
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float2 NDCPosition;
             float2 PixelPosition;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 normalWS : INTERP0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.normalWS.xyz = input.normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.normalWS = input.normalWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float2 _PlayerPosition;
        float _Size;
        float _Smoothness;
        float _Opacity;
        CBUFFER_END
        
        
        // Object and Global properties
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Remap_float2(float2 In, float2 InMinMax, float2 OutMinMax, out float2 Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_Subtract_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A - B;
        }
        
        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A / B;
        }
        
        void Unity_Length_float2(float2 In, out float Out)
        {
            Out = length(In);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }
        
        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Property_73b5f521c9a1434b94e68c9424ac3949_Out_0_Float = _Smoothness;
            float4 _ScreenPosition_acce4db933974d1996b33954074332c6_Out_0_Vector4 = float4(IN.NDCPosition.xy, 0, 0);
            float2 _Property_d2fb2bd91c8c49aa95c9b94062e6f1a0_Out_0_Vector2 = _PlayerPosition;
            float2 _Remap_ac5142f5a7444be489aeb5076ae155bf_Out_3_Vector2;
            Unity_Remap_float2(_Property_d2fb2bd91c8c49aa95c9b94062e6f1a0_Out_0_Vector2, float2 (0, 1), float2 (0.5, -1.5), _Remap_ac5142f5a7444be489aeb5076ae155bf_Out_3_Vector2);
            float2 _Add_48dac6c54ec542348fe17b42048bfe19_Out_2_Vector2;
            Unity_Add_float2((_ScreenPosition_acce4db933974d1996b33954074332c6_Out_0_Vector4.xy), _Remap_ac5142f5a7444be489aeb5076ae155bf_Out_3_Vector2, _Add_48dac6c54ec542348fe17b42048bfe19_Out_2_Vector2);
            float2 _TilingAndOffset_9589f15f948e443db747b44141f4aa02_Out_3_Vector2;
            Unity_TilingAndOffset_float((_ScreenPosition_acce4db933974d1996b33954074332c6_Out_0_Vector4.xy), float2 (1, 1), _Add_48dac6c54ec542348fe17b42048bfe19_Out_2_Vector2, _TilingAndOffset_9589f15f948e443db747b44141f4aa02_Out_3_Vector2);
            float2 _Multiply_c5784441aa3d4c399fa12fc8ce686df9_Out_2_Vector2;
            Unity_Multiply_float2_float2(_TilingAndOffset_9589f15f948e443db747b44141f4aa02_Out_3_Vector2, float2(2, 2), _Multiply_c5784441aa3d4c399fa12fc8ce686df9_Out_2_Vector2);
            float2 _Subtract_7d595324968a4210ab4a41d20f8c0acd_Out_2_Vector2;
            Unity_Subtract_float2(_Multiply_c5784441aa3d4c399fa12fc8ce686df9_Out_2_Vector2, float2(1, 1), _Subtract_7d595324968a4210ab4a41d20f8c0acd_Out_2_Vector2);
            float _Divide_f9fda8aa789847349ef190dff711f8e2_Out_2_Float;
            Unity_Divide_float(unity_OrthoParams.y, unity_OrthoParams.x, _Divide_f9fda8aa789847349ef190dff711f8e2_Out_2_Float);
            float _Property_b22295ca229a4619a90f4628fcffdc80_Out_0_Float = _Size;
            float _Multiply_2c54520d2f6243389b8b126337cd5264_Out_2_Float;
            Unity_Multiply_float_float(_Divide_f9fda8aa789847349ef190dff711f8e2_Out_2_Float, _Property_b22295ca229a4619a90f4628fcffdc80_Out_0_Float, _Multiply_2c54520d2f6243389b8b126337cd5264_Out_2_Float);
            float2 _Vector2_e090312b2c9a40c3977d9897847677e5_Out_0_Vector2 = float2(_Multiply_2c54520d2f6243389b8b126337cd5264_Out_2_Float, _Property_b22295ca229a4619a90f4628fcffdc80_Out_0_Float);
            float2 _Divide_9bfc6c9c2f0d47a4b3c5a7f3c0543bbd_Out_2_Vector2;
            Unity_Divide_float2(_Subtract_7d595324968a4210ab4a41d20f8c0acd_Out_2_Vector2, _Vector2_e090312b2c9a40c3977d9897847677e5_Out_0_Vector2, _Divide_9bfc6c9c2f0d47a4b3c5a7f3c0543bbd_Out_2_Vector2);
            float _Length_0ab2951ca39b4bb4849df4df429e6289_Out_1_Float;
            Unity_Length_float2(_Divide_9bfc6c9c2f0d47a4b3c5a7f3c0543bbd_Out_2_Vector2, _Length_0ab2951ca39b4bb4849df4df429e6289_Out_1_Float);
            float _OneMinus_7c9727e510c64c68b4a5b95359a257cc_Out_1_Float;
            Unity_OneMinus_float(_Length_0ab2951ca39b4bb4849df4df429e6289_Out_1_Float, _OneMinus_7c9727e510c64c68b4a5b95359a257cc_Out_1_Float);
            float _Saturate_0f4422edb5f74268aa6a263c205182ee_Out_1_Float;
            Unity_Saturate_float(_OneMinus_7c9727e510c64c68b4a5b95359a257cc_Out_1_Float, _Saturate_0f4422edb5f74268aa6a263c205182ee_Out_1_Float);
            float _Smoothstep_6bcd6f29d62a46efa8684f71124ef46e_Out_3_Float;
            Unity_Smoothstep_float(float(0), _Property_73b5f521c9a1434b94e68c9424ac3949_Out_0_Float, _Saturate_0f4422edb5f74268aa6a263c205182ee_Out_1_Float, _Smoothstep_6bcd6f29d62a46efa8684f71124ef46e_Out_3_Float);
            float _Property_8d1db5cc03694ac69a1992f804d0d44f_Out_0_Float = _Opacity;
            float _Multiply_e624193bbe004f1a83454d49be3168fb_Out_2_Float;
            Unity_Multiply_float_float(_Smoothstep_6bcd6f29d62a46efa8684f71124ef46e_Out_3_Float, _Property_8d1db5cc03694ac69a1992f804d0d44f_Out_0_Float, _Multiply_e624193bbe004f1a83454d49be3168fb_Out_2_Float);
            float _OneMinus_930a063f193c439bacd5f0fef9ba15de_Out_1_Float;
            Unity_OneMinus_float(_Multiply_e624193bbe004f1a83454d49be3168fb_Out_2_Float, _OneMinus_930a063f193c439bacd5f0fef9ba15de_Out_1_Float);
            surface.Alpha = _OneMinus_930a063f193c439bacd5f0fef9ba15de_Out_1_Float;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x < 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
            #else
            output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x > 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
            #endif
        
            output.NDCPosition = output.PixelPosition.xy / _ScaledScreenParams.xy;
            output.NDCPosition.y = 1.0f - output.NDCPosition.y;
        
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "GBuffer"
            Tags
            {
                "LightMode" = "UniversalGBuffer"
            }
        
        // Render State
        Cull Back
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 4.5
        #pragma exclude_renderers gles gles3 glcore
        #pragma multi_compile_instancing
        #pragma multi_compile_fog
        #pragma instancing_options renderinglayer
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
        #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_GBUFFER
        #define _SURFACE_TYPE_TRANSPARENT 1
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
            #if !defined(LIGHTMAP_ON)
             float3 sh;
            #endif
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float2 NDCPosition;
             float2 PixelPosition;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
            #if !defined(LIGHTMAP_ON)
             float3 sh : INTERP0;
            #endif
             float3 positionWS : INTERP1;
             float3 normalWS : INTERP2;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            #if !defined(LIGHTMAP_ON)
            output.sh = input.sh;
            #endif
            output.positionWS.xyz = input.positionWS;
            output.normalWS.xyz = input.normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            #if !defined(LIGHTMAP_ON)
            output.sh = input.sh;
            #endif
            output.positionWS = input.positionWS.xyz;
            output.normalWS = input.normalWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float2 _PlayerPosition;
        float _Size;
        float _Smoothness;
        float _Opacity;
        CBUFFER_END
        
        
        // Object and Global properties
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Remap_float2(float2 In, float2 InMinMax, float2 OutMinMax, out float2 Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_Subtract_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A - B;
        }
        
        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A / B;
        }
        
        void Unity_Length_float2(float2 In, out float Out)
        {
            Out = length(In);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }
        
        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Property_73b5f521c9a1434b94e68c9424ac3949_Out_0_Float = _Smoothness;
            float4 _ScreenPosition_acce4db933974d1996b33954074332c6_Out_0_Vector4 = float4(IN.NDCPosition.xy, 0, 0);
            float2 _Property_d2fb2bd91c8c49aa95c9b94062e6f1a0_Out_0_Vector2 = _PlayerPosition;
            float2 _Remap_ac5142f5a7444be489aeb5076ae155bf_Out_3_Vector2;
            Unity_Remap_float2(_Property_d2fb2bd91c8c49aa95c9b94062e6f1a0_Out_0_Vector2, float2 (0, 1), float2 (0.5, -1.5), _Remap_ac5142f5a7444be489aeb5076ae155bf_Out_3_Vector2);
            float2 _Add_48dac6c54ec542348fe17b42048bfe19_Out_2_Vector2;
            Unity_Add_float2((_ScreenPosition_acce4db933974d1996b33954074332c6_Out_0_Vector4.xy), _Remap_ac5142f5a7444be489aeb5076ae155bf_Out_3_Vector2, _Add_48dac6c54ec542348fe17b42048bfe19_Out_2_Vector2);
            float2 _TilingAndOffset_9589f15f948e443db747b44141f4aa02_Out_3_Vector2;
            Unity_TilingAndOffset_float((_ScreenPosition_acce4db933974d1996b33954074332c6_Out_0_Vector4.xy), float2 (1, 1), _Add_48dac6c54ec542348fe17b42048bfe19_Out_2_Vector2, _TilingAndOffset_9589f15f948e443db747b44141f4aa02_Out_3_Vector2);
            float2 _Multiply_c5784441aa3d4c399fa12fc8ce686df9_Out_2_Vector2;
            Unity_Multiply_float2_float2(_TilingAndOffset_9589f15f948e443db747b44141f4aa02_Out_3_Vector2, float2(2, 2), _Multiply_c5784441aa3d4c399fa12fc8ce686df9_Out_2_Vector2);
            float2 _Subtract_7d595324968a4210ab4a41d20f8c0acd_Out_2_Vector2;
            Unity_Subtract_float2(_Multiply_c5784441aa3d4c399fa12fc8ce686df9_Out_2_Vector2, float2(1, 1), _Subtract_7d595324968a4210ab4a41d20f8c0acd_Out_2_Vector2);
            float _Divide_f9fda8aa789847349ef190dff711f8e2_Out_2_Float;
            Unity_Divide_float(unity_OrthoParams.y, unity_OrthoParams.x, _Divide_f9fda8aa789847349ef190dff711f8e2_Out_2_Float);
            float _Property_b22295ca229a4619a90f4628fcffdc80_Out_0_Float = _Size;
            float _Multiply_2c54520d2f6243389b8b126337cd5264_Out_2_Float;
            Unity_Multiply_float_float(_Divide_f9fda8aa789847349ef190dff711f8e2_Out_2_Float, _Property_b22295ca229a4619a90f4628fcffdc80_Out_0_Float, _Multiply_2c54520d2f6243389b8b126337cd5264_Out_2_Float);
            float2 _Vector2_e090312b2c9a40c3977d9897847677e5_Out_0_Vector2 = float2(_Multiply_2c54520d2f6243389b8b126337cd5264_Out_2_Float, _Property_b22295ca229a4619a90f4628fcffdc80_Out_0_Float);
            float2 _Divide_9bfc6c9c2f0d47a4b3c5a7f3c0543bbd_Out_2_Vector2;
            Unity_Divide_float2(_Subtract_7d595324968a4210ab4a41d20f8c0acd_Out_2_Vector2, _Vector2_e090312b2c9a40c3977d9897847677e5_Out_0_Vector2, _Divide_9bfc6c9c2f0d47a4b3c5a7f3c0543bbd_Out_2_Vector2);
            float _Length_0ab2951ca39b4bb4849df4df429e6289_Out_1_Float;
            Unity_Length_float2(_Divide_9bfc6c9c2f0d47a4b3c5a7f3c0543bbd_Out_2_Vector2, _Length_0ab2951ca39b4bb4849df4df429e6289_Out_1_Float);
            float _OneMinus_7c9727e510c64c68b4a5b95359a257cc_Out_1_Float;
            Unity_OneMinus_float(_Length_0ab2951ca39b4bb4849df4df429e6289_Out_1_Float, _OneMinus_7c9727e510c64c68b4a5b95359a257cc_Out_1_Float);
            float _Saturate_0f4422edb5f74268aa6a263c205182ee_Out_1_Float;
            Unity_Saturate_float(_OneMinus_7c9727e510c64c68b4a5b95359a257cc_Out_1_Float, _Saturate_0f4422edb5f74268aa6a263c205182ee_Out_1_Float);
            float _Smoothstep_6bcd6f29d62a46efa8684f71124ef46e_Out_3_Float;
            Unity_Smoothstep_float(float(0), _Property_73b5f521c9a1434b94e68c9424ac3949_Out_0_Float, _Saturate_0f4422edb5f74268aa6a263c205182ee_Out_1_Float, _Smoothstep_6bcd6f29d62a46efa8684f71124ef46e_Out_3_Float);
            float _Property_8d1db5cc03694ac69a1992f804d0d44f_Out_0_Float = _Opacity;
            float _Multiply_e624193bbe004f1a83454d49be3168fb_Out_2_Float;
            Unity_Multiply_float_float(_Smoothstep_6bcd6f29d62a46efa8684f71124ef46e_Out_3_Float, _Property_8d1db5cc03694ac69a1992f804d0d44f_Out_0_Float, _Multiply_e624193bbe004f1a83454d49be3168fb_Out_2_Float);
            float _OneMinus_930a063f193c439bacd5f0fef9ba15de_Out_1_Float;
            Unity_OneMinus_float(_Multiply_e624193bbe004f1a83454d49be3168fb_Out_2_Float, _OneMinus_930a063f193c439bacd5f0fef9ba15de_Out_1_Float);
            surface.BaseColor = IsGammaSpace() ? float3(0.5, 0.5, 0.5) : SRGBToLinear(float3(0.5, 0.5, 0.5));
            surface.Alpha = _OneMinus_930a063f193c439bacd5f0fef9ba15de_Out_1_Float;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x < 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
            #else
            output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x > 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
            #endif
        
            output.NDCPosition = output.PixelPosition.xy / _ScaledScreenParams.xy;
            output.NDCPosition.y = 1.0f - output.NDCPosition.y;
        
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitGBufferPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "SceneSelectionPass"
            Tags
            {
                "LightMode" = "SceneSelectionPass"
            }
        
        // Render State
        Cull Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
        #define SCENESELECTIONPASS 1
        #define ALPHA_CLIP_THRESHOLD 1
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float2 NDCPosition;
             float2 PixelPosition;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float2 _PlayerPosition;
        float _Size;
        float _Smoothness;
        float _Opacity;
        CBUFFER_END
        
        
        // Object and Global properties
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Remap_float2(float2 In, float2 InMinMax, float2 OutMinMax, out float2 Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_Subtract_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A - B;
        }
        
        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A / B;
        }
        
        void Unity_Length_float2(float2 In, out float Out)
        {
            Out = length(In);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }
        
        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Property_73b5f521c9a1434b94e68c9424ac3949_Out_0_Float = _Smoothness;
            float4 _ScreenPosition_acce4db933974d1996b33954074332c6_Out_0_Vector4 = float4(IN.NDCPosition.xy, 0, 0);
            float2 _Property_d2fb2bd91c8c49aa95c9b94062e6f1a0_Out_0_Vector2 = _PlayerPosition;
            float2 _Remap_ac5142f5a7444be489aeb5076ae155bf_Out_3_Vector2;
            Unity_Remap_float2(_Property_d2fb2bd91c8c49aa95c9b94062e6f1a0_Out_0_Vector2, float2 (0, 1), float2 (0.5, -1.5), _Remap_ac5142f5a7444be489aeb5076ae155bf_Out_3_Vector2);
            float2 _Add_48dac6c54ec542348fe17b42048bfe19_Out_2_Vector2;
            Unity_Add_float2((_ScreenPosition_acce4db933974d1996b33954074332c6_Out_0_Vector4.xy), _Remap_ac5142f5a7444be489aeb5076ae155bf_Out_3_Vector2, _Add_48dac6c54ec542348fe17b42048bfe19_Out_2_Vector2);
            float2 _TilingAndOffset_9589f15f948e443db747b44141f4aa02_Out_3_Vector2;
            Unity_TilingAndOffset_float((_ScreenPosition_acce4db933974d1996b33954074332c6_Out_0_Vector4.xy), float2 (1, 1), _Add_48dac6c54ec542348fe17b42048bfe19_Out_2_Vector2, _TilingAndOffset_9589f15f948e443db747b44141f4aa02_Out_3_Vector2);
            float2 _Multiply_c5784441aa3d4c399fa12fc8ce686df9_Out_2_Vector2;
            Unity_Multiply_float2_float2(_TilingAndOffset_9589f15f948e443db747b44141f4aa02_Out_3_Vector2, float2(2, 2), _Multiply_c5784441aa3d4c399fa12fc8ce686df9_Out_2_Vector2);
            float2 _Subtract_7d595324968a4210ab4a41d20f8c0acd_Out_2_Vector2;
            Unity_Subtract_float2(_Multiply_c5784441aa3d4c399fa12fc8ce686df9_Out_2_Vector2, float2(1, 1), _Subtract_7d595324968a4210ab4a41d20f8c0acd_Out_2_Vector2);
            float _Divide_f9fda8aa789847349ef190dff711f8e2_Out_2_Float;
            Unity_Divide_float(unity_OrthoParams.y, unity_OrthoParams.x, _Divide_f9fda8aa789847349ef190dff711f8e2_Out_2_Float);
            float _Property_b22295ca229a4619a90f4628fcffdc80_Out_0_Float = _Size;
            float _Multiply_2c54520d2f6243389b8b126337cd5264_Out_2_Float;
            Unity_Multiply_float_float(_Divide_f9fda8aa789847349ef190dff711f8e2_Out_2_Float, _Property_b22295ca229a4619a90f4628fcffdc80_Out_0_Float, _Multiply_2c54520d2f6243389b8b126337cd5264_Out_2_Float);
            float2 _Vector2_e090312b2c9a40c3977d9897847677e5_Out_0_Vector2 = float2(_Multiply_2c54520d2f6243389b8b126337cd5264_Out_2_Float, _Property_b22295ca229a4619a90f4628fcffdc80_Out_0_Float);
            float2 _Divide_9bfc6c9c2f0d47a4b3c5a7f3c0543bbd_Out_2_Vector2;
            Unity_Divide_float2(_Subtract_7d595324968a4210ab4a41d20f8c0acd_Out_2_Vector2, _Vector2_e090312b2c9a40c3977d9897847677e5_Out_0_Vector2, _Divide_9bfc6c9c2f0d47a4b3c5a7f3c0543bbd_Out_2_Vector2);
            float _Length_0ab2951ca39b4bb4849df4df429e6289_Out_1_Float;
            Unity_Length_float2(_Divide_9bfc6c9c2f0d47a4b3c5a7f3c0543bbd_Out_2_Vector2, _Length_0ab2951ca39b4bb4849df4df429e6289_Out_1_Float);
            float _OneMinus_7c9727e510c64c68b4a5b95359a257cc_Out_1_Float;
            Unity_OneMinus_float(_Length_0ab2951ca39b4bb4849df4df429e6289_Out_1_Float, _OneMinus_7c9727e510c64c68b4a5b95359a257cc_Out_1_Float);
            float _Saturate_0f4422edb5f74268aa6a263c205182ee_Out_1_Float;
            Unity_Saturate_float(_OneMinus_7c9727e510c64c68b4a5b95359a257cc_Out_1_Float, _Saturate_0f4422edb5f74268aa6a263c205182ee_Out_1_Float);
            float _Smoothstep_6bcd6f29d62a46efa8684f71124ef46e_Out_3_Float;
            Unity_Smoothstep_float(float(0), _Property_73b5f521c9a1434b94e68c9424ac3949_Out_0_Float, _Saturate_0f4422edb5f74268aa6a263c205182ee_Out_1_Float, _Smoothstep_6bcd6f29d62a46efa8684f71124ef46e_Out_3_Float);
            float _Property_8d1db5cc03694ac69a1992f804d0d44f_Out_0_Float = _Opacity;
            float _Multiply_e624193bbe004f1a83454d49be3168fb_Out_2_Float;
            Unity_Multiply_float_float(_Smoothstep_6bcd6f29d62a46efa8684f71124ef46e_Out_3_Float, _Property_8d1db5cc03694ac69a1992f804d0d44f_Out_0_Float, _Multiply_e624193bbe004f1a83454d49be3168fb_Out_2_Float);
            float _OneMinus_930a063f193c439bacd5f0fef9ba15de_Out_1_Float;
            Unity_OneMinus_float(_Multiply_e624193bbe004f1a83454d49be3168fb_Out_2_Float, _OneMinus_930a063f193c439bacd5f0fef9ba15de_Out_1_Float);
            surface.Alpha = _OneMinus_930a063f193c439bacd5f0fef9ba15de_Out_1_Float;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x < 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
            #else
            output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x > 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
            #endif
        
            output.NDCPosition = output.PixelPosition.xy / _ScaledScreenParams.xy;
            output.NDCPosition.y = 1.0f - output.NDCPosition.y;
        
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "ScenePickingPass"
            Tags
            {
                "LightMode" = "Picking"
            }
        
        // Render State
        Cull Back
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
        #define SCENEPICKINGPASS 1
        #define ALPHA_CLIP_THRESHOLD 1
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRenderingKeywords.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/FoveatedRendering.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float2 NDCPosition;
             float2 PixelPosition;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float2 _PlayerPosition;
        float _Size;
        float _Smoothness;
        float _Opacity;
        CBUFFER_END
        
        
        // Object and Global properties
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Remap_float2(float2 In, float2 InMinMax, float2 OutMinMax, out float2 Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }
        
        void Unity_Add_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A + B;
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        void Unity_Multiply_float2_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A * B;
        }
        
        void Unity_Subtract_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A - B;
        }
        
        void Unity_Divide_float(float A, float B, out float Out)
        {
            Out = A / B;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Divide_float2(float2 A, float2 B, out float2 Out)
        {
            Out = A / B;
        }
        
        void Unity_Length_float2(float2 In, out float Out)
        {
            Out = length(In);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Saturate_float(float In, out float Out)
        {
            Out = saturate(In);
        }
        
        void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
        {
            Out = smoothstep(Edge1, Edge2, In);
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            float _Property_73b5f521c9a1434b94e68c9424ac3949_Out_0_Float = _Smoothness;
            float4 _ScreenPosition_acce4db933974d1996b33954074332c6_Out_0_Vector4 = float4(IN.NDCPosition.xy, 0, 0);
            float2 _Property_d2fb2bd91c8c49aa95c9b94062e6f1a0_Out_0_Vector2 = _PlayerPosition;
            float2 _Remap_ac5142f5a7444be489aeb5076ae155bf_Out_3_Vector2;
            Unity_Remap_float2(_Property_d2fb2bd91c8c49aa95c9b94062e6f1a0_Out_0_Vector2, float2 (0, 1), float2 (0.5, -1.5), _Remap_ac5142f5a7444be489aeb5076ae155bf_Out_3_Vector2);
            float2 _Add_48dac6c54ec542348fe17b42048bfe19_Out_2_Vector2;
            Unity_Add_float2((_ScreenPosition_acce4db933974d1996b33954074332c6_Out_0_Vector4.xy), _Remap_ac5142f5a7444be489aeb5076ae155bf_Out_3_Vector2, _Add_48dac6c54ec542348fe17b42048bfe19_Out_2_Vector2);
            float2 _TilingAndOffset_9589f15f948e443db747b44141f4aa02_Out_3_Vector2;
            Unity_TilingAndOffset_float((_ScreenPosition_acce4db933974d1996b33954074332c6_Out_0_Vector4.xy), float2 (1, 1), _Add_48dac6c54ec542348fe17b42048bfe19_Out_2_Vector2, _TilingAndOffset_9589f15f948e443db747b44141f4aa02_Out_3_Vector2);
            float2 _Multiply_c5784441aa3d4c399fa12fc8ce686df9_Out_2_Vector2;
            Unity_Multiply_float2_float2(_TilingAndOffset_9589f15f948e443db747b44141f4aa02_Out_3_Vector2, float2(2, 2), _Multiply_c5784441aa3d4c399fa12fc8ce686df9_Out_2_Vector2);
            float2 _Subtract_7d595324968a4210ab4a41d20f8c0acd_Out_2_Vector2;
            Unity_Subtract_float2(_Multiply_c5784441aa3d4c399fa12fc8ce686df9_Out_2_Vector2, float2(1, 1), _Subtract_7d595324968a4210ab4a41d20f8c0acd_Out_2_Vector2);
            float _Divide_f9fda8aa789847349ef190dff711f8e2_Out_2_Float;
            Unity_Divide_float(unity_OrthoParams.y, unity_OrthoParams.x, _Divide_f9fda8aa789847349ef190dff711f8e2_Out_2_Float);
            float _Property_b22295ca229a4619a90f4628fcffdc80_Out_0_Float = _Size;
            float _Multiply_2c54520d2f6243389b8b126337cd5264_Out_2_Float;
            Unity_Multiply_float_float(_Divide_f9fda8aa789847349ef190dff711f8e2_Out_2_Float, _Property_b22295ca229a4619a90f4628fcffdc80_Out_0_Float, _Multiply_2c54520d2f6243389b8b126337cd5264_Out_2_Float);
            float2 _Vector2_e090312b2c9a40c3977d9897847677e5_Out_0_Vector2 = float2(_Multiply_2c54520d2f6243389b8b126337cd5264_Out_2_Float, _Property_b22295ca229a4619a90f4628fcffdc80_Out_0_Float);
            float2 _Divide_9bfc6c9c2f0d47a4b3c5a7f3c0543bbd_Out_2_Vector2;
            Unity_Divide_float2(_Subtract_7d595324968a4210ab4a41d20f8c0acd_Out_2_Vector2, _Vector2_e090312b2c9a40c3977d9897847677e5_Out_0_Vector2, _Divide_9bfc6c9c2f0d47a4b3c5a7f3c0543bbd_Out_2_Vector2);
            float _Length_0ab2951ca39b4bb4849df4df429e6289_Out_1_Float;
            Unity_Length_float2(_Divide_9bfc6c9c2f0d47a4b3c5a7f3c0543bbd_Out_2_Vector2, _Length_0ab2951ca39b4bb4849df4df429e6289_Out_1_Float);
            float _OneMinus_7c9727e510c64c68b4a5b95359a257cc_Out_1_Float;
            Unity_OneMinus_float(_Length_0ab2951ca39b4bb4849df4df429e6289_Out_1_Float, _OneMinus_7c9727e510c64c68b4a5b95359a257cc_Out_1_Float);
            float _Saturate_0f4422edb5f74268aa6a263c205182ee_Out_1_Float;
            Unity_Saturate_float(_OneMinus_7c9727e510c64c68b4a5b95359a257cc_Out_1_Float, _Saturate_0f4422edb5f74268aa6a263c205182ee_Out_1_Float);
            float _Smoothstep_6bcd6f29d62a46efa8684f71124ef46e_Out_3_Float;
            Unity_Smoothstep_float(float(0), _Property_73b5f521c9a1434b94e68c9424ac3949_Out_0_Float, _Saturate_0f4422edb5f74268aa6a263c205182ee_Out_1_Float, _Smoothstep_6bcd6f29d62a46efa8684f71124ef46e_Out_3_Float);
            float _Property_8d1db5cc03694ac69a1992f804d0d44f_Out_0_Float = _Opacity;
            float _Multiply_e624193bbe004f1a83454d49be3168fb_Out_2_Float;
            Unity_Multiply_float_float(_Smoothstep_6bcd6f29d62a46efa8684f71124ef46e_Out_3_Float, _Property_8d1db5cc03694ac69a1992f804d0d44f_Out_0_Float, _Multiply_e624193bbe004f1a83454d49be3168fb_Out_2_Float);
            float _OneMinus_930a063f193c439bacd5f0fef9ba15de_Out_1_Float;
            Unity_OneMinus_float(_Multiply_e624193bbe004f1a83454d49be3168fb_Out_2_Float, _OneMinus_930a063f193c439bacd5f0fef9ba15de_Out_1_Float);
            surface.Alpha = _OneMinus_930a063f193c439bacd5f0fef9ba15de_Out_1_Float;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x < 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
            #else
            output.PixelPosition = float2(input.positionCS.x, (_ProjectionParams.x > 0) ? (_ScaledScreenParams.y - input.positionCS.y) : input.positionCS.y);
            #endif
        
            output.NDCPosition = output.PixelPosition.xy / _ScaledScreenParams.xy;
            output.NDCPosition.y = 1.0f - output.NDCPosition.y;
        
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
    }
    CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
    CustomEditorForRenderPipeline "UnityEditor.ShaderGraphUnlitGUI" "UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset"
    FallBack "Hidden/Shader Graph/FallbackError"
}