using System.Collections;
using System.Text;
// using Bolt;
// using Unity.VisualScripting;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ET
{
    [ExecuteInEditMode]
    public class TextBubble : MonoBehaviour
    {
        [SerializeField] protected TextMeshPro tmp;

        [SerializeField] protected SpriteRenderer spriteRenderer;

        [SerializeField] protected float marginX;

        [SerializeField] protected float marginY;
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            if (Application.isEditor)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(tmp.rectTransform);
                Vector2 autoSize = new Vector2(tmp.rectTransform.rect.width, tmp.rectTransform.rect.height);
                UpdateSpriteSize(autoSize);
            }
        
        }

        public void SetColor(Color color)
        {
            tmp.color = color;
            //其实可以用tween做闪字，或者颜色不断变化的字
        }

        public void Speak(string text)
        {
            ShowBubble();
            var formatted = SpeakFormat(text, 56);
            tmp.SetText(formatted);
            LayoutRebuilder.ForceRebuildLayoutImmediate(tmp.rectTransform);
            Vector2 autoSize = new Vector2(tmp.rectTransform.rect.width, tmp.rectTransform.rect.height);
            
            UpdateSpriteSize(autoSize);
            // StartCoroutine(DisappearAfter(5f));
            
        }

        /// <summary>
        /// 用来把raw text处理为适合在气泡中显示的样子
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string SpeakFormat(string text,int line_len)
        {
            StringBuilder builder = new StringBuilder();
            StringReader reader = new StringReader(text);
            string str = reader.GetByWidth(line_len);
            int lines = 0;
            while (str != null && lines<=0)
            {
                builder.AppendLine(str);
                lines++;
                str = reader.GetByWidth(line_len);
            }

            return builder.ToString();
            // StringBuilder content = new StringBuilder();
            // int buffer_size = 4;
            //
            // TextGenerator textGen = new TextGenerator();
            // TextGenerationSettings genSetting = new TextGenerationSettings();
            // genSetting.font= Font.CreateDynamicFontFromOSFont("微软雅黑", 1);
            // //
            //
            // for (int i = 0; i < text.Length; i += buffer_size)
            // {
            //     var step_size = buffer_size;
            //     if (text.Length < i + buffer_size)
            //     {
            //         step_size = text.Length - i;
            //     }
            //     var cut = text.Substring(i, step_size);
            //     var textWidth = textGen.GetPreferredWidth(cut, genSetting);
            //     content.AppendLine($"{cut}: {textWidth}");
            // }
            
            
            
            // text.Substring(0, 5)
            // var bytes = Encoding.UTF32.GetBytes(text);
            // for (int i = 0; i < bytes.Length; i++)
            // {
            //     content.Append(bytes[i]);
            // }
            // return content.ToString();
        }

        public void UpdateSpriteSize(Vector2 autoSize)
        {
            Vector2 bubbleSize = autoSize * 3f + new Vector2(marginX *2f, marginY);
            bubbleSize.y = spriteRenderer.size.y;
            spriteRenderer.size = bubbleSize;
        }

        public IEnumerator DisappearAfter(float time)
        {
            yield return new WaitForSeconds(time);
            HideBubble();
        }

        public void HideBubble()
        {
            tmp.gameObject.SetActive(false);
            spriteRenderer.gameObject.SetActive(false);
            
        }

        private void ShowBubble()
        {
            tmp.gameObject.SetActive(true);
            spriteRenderer.gameObject.SetActive(true);
        }
    }
}
