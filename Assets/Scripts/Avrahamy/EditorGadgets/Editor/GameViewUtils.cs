using UnityEditor;
using System.Reflection;

namespace Avrahamy.EditorGadgets {
    public static class GameViewUtils {
        public enum GameViewSizeType {
            AspectRatio,
            FixedResolution
        }

        private static readonly object gameViewSizesInstance;
        private static readonly MethodInfo getGroup;

        static GameViewUtils() {
            var sizesType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizes");
            var singleType = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
            var instanceProp = singleType.GetProperty("instance");
            getGroup = sizesType.GetMethod("GetGroup");
            gameViewSizesInstance = instanceProp.GetValue(null, null);
        }

        public static void AddCustomSize(GameViewSizeType viewSizeType, GameViewSizeGroupType sizeGroupType, int width, int height, string resolutionName) {
            var group = GetGroup(sizeGroupType);
            var addCustomSize = getGroup.ReturnType.GetMethod("AddCustomSize"); // or group.GetType().
            var assembly = Assembly.Load("UnityEditor.dll");
            var gameViewSize = assembly.GetType("UnityEditor.GameViewSize");
            var gameViewSizeType = assembly.GetType("UnityEditor.GameViewSizeType");
            var ctor = gameViewSize.GetConstructor(new [] {
                gameViewSizeType,
                typeof(int),
                typeof(int),
                typeof(string)
            });
            var newSize = ctor.Invoke(new object[] { (int)viewSizeType, width, height, resolutionName });
            addCustomSize.Invoke(group, new [] { newSize });
        }

        public static void SetSize(int index) {
            var gvWndType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
            var selectedSizeIndexProp = gvWndType.GetProperty("selectedSizeIndex",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var gvWnd = EditorWindow.GetWindow(gvWndType);
            selectedSizeIndexProp.SetValue(gvWnd, index, null);
        }

        public static bool SizeExists(GameViewSizeGroupType sizeGroupType, string text) {
            return FindSize(sizeGroupType, text) != -1;
        }

        public static int FindSize(GameViewSizeGroupType sizeGroupType, string text) {
            // GameViewSizes group = gameViewSizesInstance.GetGroup(sizeGroupType);
            // string[] texts = group.GetDisplayTexts();
            // for loop...

            var group = GetGroup(sizeGroupType);
            var getDisplayTexts = group.GetType().GetMethod("GetDisplayTexts");
            var displayTexts = getDisplayTexts.Invoke(group, null) as string[];
            for(int i = 0; i < displayTexts.Length; i++) {
                string display = displayTexts[i];
                // the text we get is "Name (W:H)" if the size has a name, or just "W:H" e.g. 16:9
                // so if we're querying a custom size text we substring to only get the name
                // You could see the outputs by just logging
                // Debug.Log(display);
                int pren = display.IndexOf('(');
                if (pren != -1)
                    display = display.Substring(0, pren - 1); // -1 to remove the space that's before the prens. This is very implementation-dependant
                if (display == text)
                    return i;
            }
            return -1;
        }

        public static bool SizeExists(GameViewSizeGroupType sizeGroupType, int width, int height) {
            return FindSize(sizeGroupType, width, height) != -1;
        }

        public static int FindSize(GameViewSizeGroupType sizeGroupType, int width, int height) {
            // goal:
            // GameViewSizes group = gameViewSizesInstance.GetGroup(sizeGroupType);
            // int sizesCount = group.GetBuiltinCount() + group.GetCustomCount();
            // iterate through the sizes via group.GetGameViewSize(int index)

            var group = GetGroup(sizeGroupType);
            var groupType = group.GetType();
            var getBuiltinCount = groupType.GetMethod("GetBuiltinCount");
            var getCustomCount = groupType.GetMethod("GetCustomCount");
            int sizesCount = (int)getBuiltinCount.Invoke(group, null) + (int)getCustomCount.Invoke(group, null);
            var getGameViewSize = groupType.GetMethod("GetGameViewSize");
            var gvsType = getGameViewSize.ReturnType;
            var widthProp = gvsType.GetProperty("width");
            var heightProp = gvsType.GetProperty("height");
            var indexValue = new object[1];
            for(int i = 0; i < sizesCount; i++) {
                indexValue[0] = i;
                var size = getGameViewSize.Invoke(group, indexValue);
                int sizeWidth = (int)widthProp.GetValue(size, null);
                int sizeHeight = (int)heightProp.GetValue(size, null);
                if (sizeWidth == width && sizeHeight == height)
                    return i;
            }
            return -1;
        }

        private static object GetGroup(GameViewSizeGroupType type) {
            return getGroup.Invoke(gameViewSizesInstance, new object[] { (int)type });
        }
    }
}
