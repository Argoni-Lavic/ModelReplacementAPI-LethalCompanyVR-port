using System.Linq;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine;

namespace ModelReplacement.Scripts
{
    public class MaterialHelper
    {
        /// <summary> Shaders with any of these prefixes won't be automatically converted. </summary>
        private static readonly string[] shaderPrefixWhitelist =
        {
            "HDRP/",
            "GUI/",
            "Sprites/",
            "UI/",
            "Unlit/",
            "Toon",
            "lilToon",
            "Shader Graphs/",
            "Hidden/"
        };

        /// <summary>
        /// Get a replacement material based on the original game material, and the material found on the replacing model.
        /// </summary>
        /// <param name="gameMaterial">The equivalent material on the model being replaced.</param>
        /// <param name="modelMaterial">The material on the replacing model.</param>
        /// <returns>The replacement material created from the <see cref="gameMaterial"/> and the <see cref="modelMaterial"/></returns>
        public virtual Material GetReplacementMaterial(Material gameMaterial, Material modelMaterial)
        {
            // 1. Regular Non-VR Logic: Check whitelist
            if (!ModelReplacementAPI.lcvrPresent && shaderPrefixWhitelist.Any(prefix => modelMaterial.shader.name.StartsWith(prefix)))
            {
                return modelMaterial;
            }

            // 2. VR Logic or Non-Whitelisted Shader: Clone the base game's safe native HDRP shader
            if (ModelReplacementAPI.lcvrPresent)
            {
                // Log once per asset setup, not flooded continuously
                ModelReplacementAPI.Instance.Logger.LogWarning($"[MAPI-LCVR] Forcing VR compatibility for {modelMaterial.name}. Textures will transfer, but advanced custom shader effects will be lost.");
            }
            else
            {
                ModelReplacementAPI.Instance.Logger.LogInfo($"Creating replacement material for non-whitelisted shader: {modelMaterial.shader.name}");
            }

            // Clone the vanilla game material (automatically inherits a VR-compatible HDRP shader)
            Material replacementMat = new Material(gameMaterial);

            // CRITICAL FOR VR STEREO RENDERING: Force GPU Instancing on the cloned material
            replacementMat.enableInstancing = true;

            // 3. Map Basic Transform & Rendering Settings
            replacementMat.color = modelMaterial.color;
            replacementMat.mainTexture = modelMaterial.mainTexture;
            replacementMat.mainTextureOffset = modelMaterial.mainTextureOffset;
            replacementMat.mainTextureScale = modelMaterial.mainTextureScale;

            // 4. Safely Extract Custom Textures from the Custom Model
            if (modelMaterial.HasTexture("_BaseColorMap")) 
            {
                replacementMat.SetTexture("_BaseColorMap", modelMaterial.GetTexture("_BaseColorMap"));
            }  
            if (modelMaterial.HasTexture("_BumpMap"))
            {
                replacementMat.SetTexture("_BumpMap", modelMaterial.GetTexture("_BumpMap"));
                replacementMat.EnableKeyword("_NORMALMAP");
                replacementMat.SetFloat("_NormalScale", 1.0f); // Changed from 0 so normals actually render
            }

            if (modelMaterial.HasTexture("_EmissiveColorMap"))
            {
                replacementMat.SetTexture("_EmissiveColorMap", modelMaterial.GetTexture("_EmissiveColorMap"));
                replacementMat.EnableKeyword("_EMISSION");
            }
            
            if (modelMaterial.HasColor("_EmissiveColor"))
            {
                replacementMat.SetColor("_EmissiveColor", modelMaterial.GetColor("_EmissiveColor"));
                replacementMat.EnableKeyword("_EMISSION");
            }

            if (modelMaterial.HasTexture("_SpecularColorMap"))
            {
                replacementMat.SetTexture("_SpecularColorMap", modelMaterial.GetTexture("_SpecularColorMap"));
                replacementMat.EnableKeyword("_SPECGLOSSMAP");
            }

            if (modelMaterial.HasFloat("_Smoothness"))
            {
                replacementMat.SetFloat("_Smoothness", modelMaterial.GetFloat("_Smoothness"));
            }

            // 5. Finalize Unity HDRP structural settings
            HDMaterial.ValidateMaterial(replacementMat);
            return replacementMat;
        }
    }
}
