using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RB_CardParser : MonoBehaviour
{
    //Structs
    public struct CardInfo
    {
        public string CardNumber;
        public string ExpiryDate;
        public string Cvv;
    }

    //Card Inputs
    [Header("Card Inputs")]
    [SerializeField] private TMP_InputField _cardNumberInput;
    [SerializeField] private TMP_InputField _expiryDateMonthInput;
    [SerializeField] private TMP_InputField _expiryDateYearInput;
    [SerializeField] private TMP_InputField _cvvInput;

    //Current card
    private CardInfo _currentCardInfo;

    //Components
    private CanvasGroup _canvasGroup;

    //Awake
    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0;
    }

    public void OpenCardMenu()
    {
        _canvasGroup.alpha = 1;
        RB_InputManager.Instance.InputEnabled = false;
        print("open");
    }

    public void CloseCardMenu()
    {
        _canvasGroup.alpha = 0;
        _cardNumberInput.DeactivateInputField();
        _expiryDateMonthInput.DeactivateInputField();
        _expiryDateYearInput.DeactivateInputField();
        _cvvInput.DeactivateInputField();
        RB_InputManager.Instance.InputEnabled = true;
    }

    public void SubmitCardInfos()
    {
        _currentCardInfo.CardNumber = _cardNumberInput.text;
        _currentCardInfo.ExpiryDate = _expiryDateMonthInput.text + " / " + _expiryDateYearInput.text;
        _currentCardInfo.Cvv = _cvvInput.text;
    }
}
