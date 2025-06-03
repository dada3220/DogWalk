using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cysharp.Threading.Tasks;
using TMPro;
using System.Collections.Generic;

public class TitleManager : MonoBehaviour
{
    // ==== ���S�֘A ====
    [SerializeField] private Canvas canvas;
    [SerializeField] private RectTransform logoRect;
    [SerializeField] private Vector2 logoTargetPosition;
    [SerializeField] private Vector2 logoTargetScale = new Vector2(0.5f, 0.5f);
    [SerializeField] private float moveDuration = 0.5f;

    // ==== UI�v�f ====
    [SerializeField] private Button startButton;
    [SerializeField] private TMP_Text bestScoreText;

    // ==== ���j���[�֘A ====
    [SerializeField] private GameObject rightMenu;
    [SerializeField] private Button gameStartButton;
    [SerializeField] private Button ruleButton;
    [SerializeField] private Button optionButton;
    [SerializeField] private Button quitButton; 
    // ==== �I�v�V������ʊ֘A ====
    [SerializeField] private GameObject optionPanel;
    [SerializeField] private Button closeOptionButton;

    // ==== ���[����ʊ֘A ====
    [SerializeField] private GameObject rulePanel;         
    [SerializeField] private Button closeRuleButton; 

    // ==== �I���m�F�_�C�A���O ====
    [SerializeField] private GameObject quitDialog;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    private void Start()
    {
        // ������ԂŔ�\���ɂ���v�f
        rightMenu.SetActive(false);
        bestScoreText.gameObject.SetActive(false);
        quitDialog.SetActive(false);
        optionPanel.SetActive(false);
        rulePanel.SetActive(false); 

        // ���S�̈ړ�����v�Z
        logoTargetPosition = CalculateLogoTargetPosition();

        // �e��{�^���̃C�x���g�o�^
        startButton.onClick.AddListener(() => OnStartButtonClicked().Forget());
        gameStartButton.onClick.AddListener(() => LoadGame().Forget());
        quitButton.onClick.AddListener(ShowQuitDialog);
        yesButton.onClick.AddListener(QuitGame);
        noButton.onClick.AddListener(HideQuitDialog);
        optionButton.onClick.AddListener(ShowOptionPanel);
        closeOptionButton.onClick.AddListener(HideOptionPanel);
        ruleButton.onClick.AddListener(ShowRulePanel);        
        closeRuleButton.onClick.AddListener(HideRulePanel);  
    }

    // ���S�̖ڕW�ʒu�i����1/4�����j����ʃT�C�Y����v�Z
    private Vector2 CalculateLogoTargetPosition()
    {
        if (logoRect == null || canvas == null)
        {
            return Vector2.zero;
        }

        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        return new Vector2(-canvasRect.rect.width / 5f, canvasRect.rect.height / 5f);
    }

    // �X�^�[�g�{�^���������̏���
    private async UniTaskVoid OnStartButtonClicked()
    {
        startButton.interactable = false;
        startButton.gameObject.SetActive(false); // �{�^�����\��

        // ���S�ړ��{�k��
        await MoveAndScaleLogo();

        // �x�X�g�X�R�A�擾�E�\��
        int bestScore = await ScoreSaveSystem.GetBestScoreAsync();
        bestScoreText.text = $"BEST SCORE \n{bestScore}";
        bestScoreText.gameObject.SetActive(true);

        // ���j���[���t�F�[�h�\��
        await ShowRightMenuAsync();
    }

    // ���S�̈ړ��{�X�P�[���ύX�A�j���[�V����
    private async UniTask MoveAndScaleLogo()
    {
        Vector2 startPos = logoRect.anchoredPosition;
        Vector2 startScale = logoRect.localScale;
        float time = 0f;

        while (time < moveDuration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / moveDuration);
            float eased = EaseInOutQuad(t);

            logoRect.anchoredPosition = Vector2.Lerp(startPos, logoTargetPosition, eased);
            logoRect.localScale = Vector2.Lerp(startScale, logoTargetScale, eased);
            await UniTask.Yield();
        }

        // �ŏI�l����������ݒ�
        logoRect.anchoredPosition = logoTargetPosition;
        logoRect.localScale = logoTargetScale;
    }

    // ���j���[�̊e�{�^�������ԂɃt�F�[�h�\��
    private async UniTask ShowRightMenuAsync()
    {
        rightMenu.SetActive(true);

        // �S�Ă̎q��CanvasGroup��ݒ肵�A������Ԃ�0�ɂ���
        var canvasGroups = new List<CanvasGroup>();

        foreach (Transform child in rightMenu.transform)
        {
            var cg = child.GetComponent<CanvasGroup>() ?? child.gameObject.AddComponent<CanvasGroup>();
            cg.alpha = 0f;
            cg.interactable = false;
            cg.blocksRaycasts = false;
            child.gameObject.SetActive(true);
            canvasGroups.Add(cg);
        }

        // �S�̂��ꊇ�Ńt�F�[�h�C��
        float fadeTime = 0.3f;
        float time = 0f;
        while (time < fadeTime)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Clamp01(time / fadeTime);

            foreach (var cg in canvasGroups)
            {
                cg.alpha = alpha;
            }

            await UniTask.Yield();
        }

        // �ŏI�I�ɑS�ėL����
        foreach (var cg in canvasGroups)
        {
            cg.alpha = 1f;
            cg.interactable = true;
            cg.blocksRaycasts = true;
        }
    }

    // �Q�[���V�[���֑J��
    private async UniTaskVoid LoadGame()
    {
        await SceneManager.LoadSceneAsync("GameScene");
    }

    // �I�v�V�����\���E��\��
    private void ShowOptionPanel()
    {
        optionPanel.SetActive(true);
    }

    private void HideOptionPanel()
    {
        optionPanel.SetActive(false);
    }

    // ���[���p�l���\��
    private void ShowRulePanel()
    {
        rulePanel.SetActive(true);
    }

    // ���[���p�l����\��
    private void HideRulePanel()
    {
        rulePanel.SetActive(false);
    }

    // �I���m�F�_�C�A���O��\��
    private void ShowQuitDialog()
    {
        quitDialog.SetActive(true);
    }

    // �_�C�A���O���\���i�L�����Z���j
    private void HideQuitDialog()
    {
        quitDialog.SetActive(false);
    }

    // ���ۂɃQ�[�����I��
    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // �G�f�B�^�Œ�~
#else
        Application.Quit(); // ���@�ŃA�v���I��
#endif
    }

    // �ɋ}�����C�[�W���O�֐�
    private float EaseInOutQuad(float t)
    {
        return t < 0.5f ? 2f * t * t : -1f + (4f - 2f * t) * t;
    }
}
