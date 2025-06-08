using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI
{
    public class UIManager: MonoBehaviour
    {
        [SerializeField] private GameObject hpPanel;

        private List<GameObject> hps;
        private string hpPrefix = "HP_";
        
        private void Start()
        {
            hps = new List<GameObject>();
            for (int i = 0; i < hpPanel.transform.childCount; i++)
            {
                hps.Add(hpPanel.transform.GetChild(i).gameObject);
            }
        }

        public void SetHpPanel(int hp)
        {
            string hpStr = hpPrefix + hp;
            for (int i = 0; i < hps.Count; i++)
            {
                if (hp <= 0)
                {
                    hps.Find(x => x.name == hpPrefix + 0).SetActive(true);
                }
                else if (hps[i].name ==hpStr)
                {
                    hps[i].SetActive(true);
                }
                else
                {
                    hps[i].SetActive(false);
                }
            }
        }
    }
}