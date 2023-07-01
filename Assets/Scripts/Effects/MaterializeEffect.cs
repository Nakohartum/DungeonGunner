using System.Collections;
using UnityEngine;

namespace Effects
{
    public class MaterializeEffect : MonoBehaviour
    {
        public IEnumerator MaterializeRoutine(Shader materializeShader, Color materializeColor, float materializeTime,
            SpriteRenderer[] spriteRendererArray, Material normalMaterial)
        {
            Material materializeMaterial = new Material(materializeShader);

            materializeMaterial.SetColor("_EmmissionColor", materializeColor);

            foreach (var spriteRenderer in spriteRendererArray)
            {
                spriteRenderer.material = materializeMaterial;
            }

            float dissolveAmount = 0;

            while (dissolveAmount < 1f)
            {
                dissolveAmount += Time.deltaTime / materializeTime;

                materializeMaterial.SetFloat("_DissolveAmount", dissolveAmount);

                yield return null;
            }
            
            foreach (var spriteRenderer in spriteRendererArray)
            {
                spriteRenderer.material = normalMaterial;
            }
        }
    }
}