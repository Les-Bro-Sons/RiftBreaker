using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

//NOT USED
public class RB_BlurEffect : ScriptableRendererFeature {
    class RB_BlurPass : ScriptableRenderPass {
        public Material BlurMaterial = null;
        public int BlurStrength = 1;

        RTHandle _source;
        RTHandle _temporaryTexture;

        public RB_BlurPass(Material material) {
            this.BlurMaterial = material;
        }

        public void Setup(RTHandle source) {
            this._source = source;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData) {
            var descriptor = renderingData.cameraData.cameraTargetDescriptor;
            descriptor.depthBufferBits = 0;

            RenderingUtils.ReAllocateIfNeeded(ref _temporaryTexture, descriptor, FilterMode.Bilinear, TextureWrapMode.Clamp, name: "_TemporaryColorTexture");
        }


        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
            CommandBuffer cmd = CommandBufferPool.Get("BlurEffect");

            Blit(cmd, _source, _temporaryTexture, BlurMaterial, 0);
            Blit(cmd, _temporaryTexture, _source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd) {
            _temporaryTexture?.Release();
        }
    }

    RB_BlurPass _blurPass;
    public Material _blurMaterial;
    public int _blurStrength;

    public override void Create() {
        _blurPass = new RB_BlurPass(_blurMaterial) {
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents
        };
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
        if (_blurMaterial != null) {
            _blurPass.Setup(renderer.cameraColorTargetHandle);
            renderer.EnqueuePass(_blurPass);
        }
    }
}
