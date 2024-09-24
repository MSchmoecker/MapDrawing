using HarmonyLib;
using UnityEngine;

namespace MapDrawing;

[HarmonyPatch]
public static class Patches {
    private static Texture2D fogTexture;
    private static bool[] explored;
    private static bool overMap;

    [HarmonyPatch(typeof(Minimap), nameof(Minimap.Start)), HarmonyPostfix]
    public static void DisableFog(Minimap __instance) {
        overMap = false;

        fogTexture = new Texture2D(__instance.m_textureSize, __instance.m_textureSize, TextureFormat.RGBA32, false);
        fogTexture.name = "_Minimap m_fogTexture";
        fogTexture.wrapMode = TextureWrapMode.Clamp;

        explored = new bool[__instance.m_textureSize * __instance.m_textureSize];

        // Color[] colors = new Color[fogTexture.width * fogTexture.height];
        // for (int i = 0; i < colors.Length; i++) {
        //     colors[i] = Color.clear;
        // }
        //
        // fogTexture.SetPixels(colors);
        // fogTexture.Apply();

        __instance.m_mapLargeShader.SetTexture("_FogTex", fogTexture);
        __instance.m_mapSmallShader.SetTexture("_FogTex", fogTexture);
        
        UIInputHandler inputHandler = __instance.m_mapImageLarge.GetComponent<UIInputHandler>();
        inputHandler.m_onPointerEnter += (handler) => overMap = true;
        inputHandler.m_onPointerExit += (handler) => overMap = false;
        inputHandler.m_onRightDown += (handler) => MapDrawing.Log.LogInfo("Right click Down");
    }

    [HarmonyPatch(typeof(Minimap), nameof(Minimap.Update)), HarmonyPostfix]
    public static void UpdateFog(Minimap __instance) {
        if (!overMap)
            return;

        if (Input.GetMouseButton(1)) {
            Vector3 worldPoint = __instance.ScreenToWorldPoint(ZInput.mousePosition);
            Explore(__instance, worldPoint, 10f);
        }
    }
    
    [HarmonyPatch(typeof(Minimap), nameof(Minimap.OnMapRightClick)), HarmonyPostfix]
    public static void DrawOnMap(Minimap __instance) {
        Vector3 worldPoint = __instance.ScreenToWorldPoint(ZInput.mousePosition);
        // __instance.WorldToMapPoint(worldPoint, out float mx, out float my);
        Explore(__instance, worldPoint, 10f);
    }

    public static void Explore(Minimap __instance, Vector3 p, float radius) {
        int num = (int)Mathf.Ceil(radius / __instance.m_pixelSize);
        bool flag = false;
        int px;
        int py;
        __instance.WorldToPixel(p, out px, out py);
        for (int y = py - num; y <= py + num; ++y) {
            for (int x = px - num; x <= px + num; ++x) {
                if (x >= 0 && y >= 0 && x < __instance.m_textureSize && y < __instance.m_textureSize && new Vector2(x - px, y - py).magnitude <= (double)num && Explore(__instance, x, y))
                    flag = true;
            }
        }

        if (!flag)
            return;
        fogTexture.Apply();
    }

    public static bool Explore(Minimap __instance, int x, int y) {
        if (explored[y * __instance.m_textureSize + x])
            return false;
        Color pixel = fogTexture.GetPixel(x, y) with {
            r = 0.0f
        };
        fogTexture.SetPixel(x, y, pixel);
        explored[y * __instance.m_textureSize + x] = true;
        return true;
    }
}
