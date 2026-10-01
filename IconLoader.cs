using BepInEx.Logging;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace HalfBeams
{
    /// <summary>
    /// Loads a piece icon embedded in the DLL as "Icons.&lt;prefab name&gt;.png".
    /// Returns null when there is no embedded icon, so the piece keeps its source piece's icon.
    /// </summary>
    internal static class IconLoader
    {
        // ImageConversion.LoadImage has an overload taking ReadOnlySpan<byte>, a type that does not exist
        // in the net472 reference assemblies, so the compiler refuses any direct call (CS0518). Looking
        // up the byte[] overload by reflection sidesteps that.
        private static readonly MethodInfo LoadImageMethod = typeof(ImageConversion).GetMethod(
            "LoadImage",
            new[] { typeof(Texture2D), typeof(byte[]) });

        public static Sprite Load(string prefabName, ManualLogSource log)
        {
            string resourceName = "Icons." + prefabName + ".png";

            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    return null;
                }

                byte[] bytes;
                using (var memory = new MemoryStream())
                {
                    stream.CopyTo(memory);
                    bytes = memory.ToArray();
                }

                var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false) { name = prefabName + "_icon" };
                bool decoded = LoadImageMethod != null
                               && (bool)LoadImageMethod.Invoke(null, new object[] { texture, bytes });
                if (!decoded)
                {
                    log.LogWarning("Could not decode icon " + resourceName);
                    return null;
                }

                return Sprite.Create(
                    texture,
                    new Rect(0f, 0f, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f));
            }
        }
    }
}
