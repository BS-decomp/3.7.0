"""Guards the Android over-exposure fix (docs/android-lightmap-overexposure.md)."""
import re
import unittest
from pathlib import Path

CLIENT = Path(__file__).resolve().parents[1] / "client"
SHADERS = CLIENT / "Assets" / "Shader"
LIGHTMAP_SHADERS = ["Mobile-Lightmap-Unlit.shader", "Mobile-VertexLit.shader",
                    "Mobile-VertexLit-OnlyDirectionalLights.shader"]


def code(name):
    """Shader source without // comments."""
    return re.sub(r"//.*", "", (SHADERS / name).read_text())


class LightmapShaderTests(unittest.TestCase):
    def test_lightmap_decode_is_explicit_dldr(self):
        for name in LIGHTMAP_SHADERS:
            text = code(name)
            self.assertNotIn("DecodeLightmap", text, name)
            self.assertNotIn("unity_LightmapMatrix", text, name)
            self.assertNotRegex(text, r"\bquad\b", name)
            self.assertRegex(text, r"2\.0h?\s*\*\s*UNITY_SAMPLE_TEX2D\(unity_Lightmap", name)

    def test_legacy_lightmap_passes_share_one_program(self):
        for name in ["Mobile-Lightmap-Unlit.shader", "Mobile-VertexLit.shader"]:
            text = code(name)
            for tag in ("VertexLM", "VertexLMRGBM"):
                self.assertRegex(text, r'(?i)"LightMode"\s*=\s*"' + tag + '"', name + tag)
            self.assertNotIn("SetTexture [unity_Lightmap]", text, name)

    def test_lightmap_textures_are_plain_textures(self):
        metas = list((CLIENT / "Assets" / "Levels").rglob("LightmapFar-0.png.meta"))
        self.assertEqual(len(metas), 54)
        for meta in metas:
            self.assertRegex(meta.read_text(), r"(?m)^  textureType: 0$", str(meta))


if __name__ == "__main__":
    unittest.main()
