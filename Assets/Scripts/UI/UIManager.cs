using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GameObject hpPanel;
        [SerializeField] private GameObject hp;

        [SerializeField] private Sprite heart_Full;
        [SerializeField] private Sprite heart_3;
        [SerializeField] private Sprite heart_2;
        [SerializeField] private Sprite heart_1;
        [SerializeField] private Sprite heart_Empty;

        [SerializeField] private TextMeshProUGUI countdownText;

        private string hpPrefix = "HP_";
        private string heartPrefix = "HeartGroup_";

        private void Start()
        {
            countdownText.gameObject.SetActive(false);
        }

        public void InitinalHpPanel(int hp)
        {
            int heartCount = Mathf.CeilToInt(hp / 4f);

            foreach (Transform child in hpPanel.transform)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < heartCount; i++)
            {
                Instantiate(this.hp, hpPanel.transform);
            }
        }

        public void SetHpPanel(int hp)
        {
            if (hp <= 0)
            {
                foreach (Transform child in hpPanel.transform)
                {
                    var image = child.GetComponent<UnityEngine.UI.Image>();
                    if (image != null)
                        image.sprite = heart_Empty;
                }
                return;
            }

            int remainingHp = hp;
            foreach (Transform child in hpPanel.transform)
            {
                var image = child.GetComponent<UnityEngine.UI.Image>();
                if (image == null) continue;

                int currentHeartHp = Mathf.Min(remainingHp, 4);

                switch (currentHeartHp)
                {
                    case 4:
                        image.sprite = heart_Full;
                        break;
                    case 3:
                        image.sprite = heart_3;
                        break;
                    case 2:
                        image.sprite = heart_2;
                        break;
                    case 1:
                        image.sprite = heart_1;
                        break;
                    default:
                        image.sprite = heart_Empty;
                        break;
                }

                remainingHp -= 4;
            }
        }

        public void ShowCountdownText(int seconds)
        {
            countdownText.gameObject.SetActive(true);
            countdownText.text = seconds.ToString();

            StartCoroutine(HideCountdownAfterDelay(1f));
        }

        private IEnumerator HideCountdownAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            countdownText.gameObject.SetActive(false);
        }
    }
}
