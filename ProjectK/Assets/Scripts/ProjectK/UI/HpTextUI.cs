using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectK
{
    public class HpTextUI : UIBase
    {
        private HpTextUIDetail detail;

        protected override void Init()
        {
            detail = GameObject.GetComponent<HpTextUIDetail>();
            detail._HpText.gameObject.SetActive(false);
        }

        protected override void OnRefresh(params object[] args)
        {
            string textStr = (string)args[0];

            GameObject textGameObject = GameObject.Instantiate(detail._HpText.gameObject);
            textGameObject.transform.SetParent(GameObject.transform, false);
            textGameObject.name = textStr;
            textGameObject.SetActive(true);
            Text text = textGameObject.GetComponent<Text>();
            text.text = textStr;
            textGameObject.AddComponent<HpText>();
        }

        private class HpText : MonoBehaviour
        {
            private Text text;
            private int currentFrame = 0;

            private void Awake()
            {
                text = gameObject.GetComponent<Text>();
            }

            private void Update()
            {
                currentFrame += 1;

                if (currentFrame <= 30)
                {
                    Color color = text.color;
                    color.a = 1 - currentFrame / 60.0f;
                    text.color = color;
                    transform.localScale = Vector3.one * (1 - currentFrame / 240.0f);
                    transform.Translate(0, 2, 0);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
