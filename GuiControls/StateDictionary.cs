using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Zen.Utilities;
using Zen.Utilities.ExtensionMethods;

namespace Zen.GuiControls
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class StateDictionary
    {
        private readonly Dictionary<string, string> _dictionary;

        public StateDictionary()
        {
            _dictionary = new Dictionary<string, string>();
        }


        public string this[string propertyName]
        {
            //get => _dictionary[propertyName];
            set
            {
                if (_dictionary.ContainsKey(propertyName))
                {
                    _dictionary[propertyName] = value;
                }
                else
                {
                    _dictionary.Add(propertyName, value);
                }
            }
        }

        public bool ContainsKey(string propertyName)
        {
            var containsKey = _dictionary.ContainsKey(propertyName);

            return containsKey;
        }

        public string GetAsString(string propertyName, string dflt)
        {
            var valueAsString = GetString(propertyName);
            if (!valueAsString.HasValue()) return dflt;
            if (!valueAsString.StartsAndEndsWith('\'')) throw new Exception($"Failed To convert [{valueAsString}] for [{propertyName}] to string.");

            var value = valueAsString.RemoveFirstAndLastCharacters();

            return value;
        }

        public int GetAsInt32(string propertyName, int dflt)
        {
            var valueAsString = GetString(propertyName);
            if (!valueAsString.HasValue()) return dflt;

            var value = Convert.ToInt32(valueAsString);

            return value;
        }

        public float GetAsSingle(string propertyName, float dflt)
        {
            var valueAsString = GetString(propertyName);
            if (!valueAsString.HasValue()) return dflt;

            var value = Convert.ToSingle(valueAsString);

            return value;
        }

        public bool GetAsBoolean(string propertyName, bool dflt)
        {
            var valueAsString = GetString(propertyName);
            if (!valueAsString.HasValue()) return dflt;

            var value = Convert.ToBoolean(valueAsString);

            return value;
        }

        public Alignment GetAsAlignment(string propertyName, Alignment dflt)
        {
            var valueAsString = GetString(propertyName);
            if (!valueAsString.HasValue()) return dflt;

            var value = TranslateAlignment(valueAsString, propertyName);

            return value;
        }

        public ParentContainerAlignment GetAsParentContainerAlignment(string propertyName, ParentContainerAlignment dflt)
        {
            var valueAsString = GetString(propertyName);
            if (!valueAsString.HasValue()) return dflt;

            var value = TranslateParentContainerAlignment(valueAsString, propertyName);

            return value;
        }

        public PointI GetAsPointI(string propertyName, PointI dflt)
        {
            var valueAsString = GetString(propertyName);
            if (!valueAsString.HasValue()) return dflt;
            if (!valueAsString.StartsAndEndsWith('[', ']')) throw new Exception($"Failed To convert [{valueAsString}] for [{propertyName}] to PointI.");

            valueAsString = valueAsString.RemoveFirstAndLastCharacters();
            var value = TranslatePointI(valueAsString, propertyName);

            return value;
        }

        public Color GetAsColor(string propertyName, Color dflt)
        {
            var valueAsString = GetString(propertyName);
            if (!valueAsString.HasValue()) return dflt;

            var value = TranslateColor(valueAsString, propertyName);

            return value;
        }

        public Func<object, string> GetAsGetTextFunc(string propertyName, string callingTypeFullName, string callingAssemblyFullName, Func<object, string> dflt)
        {
            var valueAsString = GetString(propertyName);
            if (!valueAsString.HasValue()) return dflt;

            var value = TranslateGetTextFunc(valueAsString, propertyName, callingTypeFullName, callingAssemblyFullName);

            return value;
        }

        public List<string> GetAsListOfStrings(string propertyName, List<string> dflt)
        {
            var valueAsString = GetString(propertyName);
            if (!valueAsString.HasValue()) return dflt;

            var listOfStrings = new List<string>();
            try
            {
                var valueAsString2 = valueAsString.RemoveFirstAndLastCharacters();
                var str = valueAsString2.Split(';');
                listOfStrings.AddRange(str);
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to convert [{valueAsString}] for [{propertyName}] to List<string>.", e);
            }

            return listOfStrings;
        }

        public List<PointI> GetAsListOfPointI(string propertyName, List<PointI> dflt)
        {
            var valueAsString = GetString(propertyName);
            if (!valueAsString.HasValue()) return dflt;

            var listOfPointI = new List<PointI>();
            try
            {
                var valueAsString2 = valueAsString.RemoveFirstAndLastCharacters();
                var str = valueAsString2.Split("];[");
                foreach (var s in str)
                {
                    var s2 = s.KeepOnlyAfterCharacter('[');
                    var s3 = s2.KeepOnlyBeforeCharacter(']');
                    var p = new PointI(s3);
                    listOfPointI.Add(p);
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to convert [{valueAsString}] for [{propertyName}] to List<PointI>.", e);
            }

            return listOfPointI;
        }

        public List<Texture> GetAsListOfTextures(string propertyName, string callingTypeFullName, string callingAssemblyFullName, List<Texture> dflt)
        {
            var valueAsString = GetString(propertyName);
            if (!valueAsString.HasValue()) return dflt;

            var listOfTextures = new List<Texture>();
            try
            {
                var valueAsString2 = valueAsString.RemoveFirstAndLastCharacters();
                var str = valueAsString2.Split(';');
                foreach (var s in str)
                {
                    var s2 = s.RemoveFirstAndLastCharacters();
                    var s3 = s2.Split(':');
                    var texture = new Texture(
                        s3[0],
                        s3[1],
                        TranslateIsValidFunc(s3[2], propertyName, callingTypeFullName, callingAssemblyFullName),
                        TranslateDestinationFunc(s3[3], propertyName, callingTypeFullName, callingAssemblyFullName));
                    listOfTextures.Add(texture);
                }
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to convert [{valueAsString}] for [{propertyName}] to List<Texture>.", e);
            }
            return listOfTextures;
        }

        private string GetString(string propertyName)
        {
            if (!_dictionary.ContainsKey(propertyName)) return string.Empty;
            var valueAsString = _dictionary[propertyName];

            return valueAsString;
        }

        private static Alignment TranslateAlignment(string alignmentAsString, string propertyName)
        {
            try
            {
                var success = Enum.TryParse(alignmentAsString, out Alignment alignment);
                if (!success) throw new Exception($"Alignment {alignmentAsString} could not be translated.");

                return alignment;
            }
            catch (Exception e)
            {
                throw new Exception($"Failed To convert [{alignmentAsString}] for [{propertyName}] to Alignment.", e);
            }
        }

        private static ParentContainerAlignment TranslateParentContainerAlignment(string parentContainerAlignmentAsString, string propertyName)
        {
            try
            {
                var success = Enum.TryParse(parentContainerAlignmentAsString, out ParentContainerAlignment alignment);
                if (!success) throw new Exception($"ParentContainerAlignment {parentContainerAlignmentAsString} could not be translated.");

                return alignment;
            }
            catch (Exception e)
            {
                throw new Exception($"Failed To convert [{parentContainerAlignmentAsString}] for [{propertyName}] to Alignment.", e);
            }
        }

        private static PointI TranslatePointI(string pointIAsString, string propertyName)
        {
            try
            {
                var x = Convert.ToInt32(pointIAsString.Split(';')[0].Trim());
                var y = Convert.ToInt32(pointIAsString.Split(';')[1].Trim());
                var pointI = new PointI(x, y);

                return pointI;
            }
            catch (Exception e)
            {
                throw new Exception($"Failed To convert [{pointIAsString}] for [{propertyName}] to PointI.", e);
            }
        }

        private static Color TranslateColor(string colorAsString, string propertyName)
        {
            try
            {
                var tempColor = System.Drawing.Color.FromName(colorAsString);
                var color = new Color(tempColor.R, tempColor.G, tempColor.B, tempColor.A);

                return color;
            }
            catch (Exception e)
            {
                throw new Exception($"Failed To convert [{colorAsString}] for [{propertyName}] to Color.", e);
            }
        }

        private static Func<object, string> TranslateGetTextFunc(string getTextFuncAsString, string propertyName, string callingTypeFullName, string callingAssemblyFullName) // 'Game1.EventHandlers, Game1 - GetTextFunc' or 'Game1.EventHandlers.GetTextFunc' or 'GetTextFunc'
        {
            var firstAndLastCharactersRemoved = getTextFuncAsString.RemoveFirstAndLastCharacters(); // remove single quotes

            try
            {
                var assemblyQualifiedName = GetAssemblyQualifiedName(firstAndLastCharactersRemoved, callingTypeFullName, callingAssemblyFullName);
                var methodName = GetMethodName(firstAndLastCharactersRemoved);
                var func = CreateFuncDelegate(assemblyQualifiedName, methodName);

                return func;
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to convert [{getTextFuncAsString}] for property [{propertyName}] to Func.", e);
            }
        }

        private static Func<IControl, bool> TranslateIsValidFunc(string getIsValidFuncAsString, string propertyName, string callingTypeFullName, string callingAssemblyFullName)
        {
            try
            {
                var assemblyQualifiedName = GetAssemblyQualifiedName(getIsValidFuncAsString, callingTypeFullName, callingAssemblyFullName);
                var methodName = GetMethodName(getIsValidFuncAsString);
                var func = CreateFuncDelegate2(assemblyQualifiedName, methodName);

                return func;
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to convert [{getIsValidFuncAsString}] for property [{propertyName}] to Func.", e);
            }
        }

        private static Func<IControl, Rectangle> TranslateDestinationFunc(string getDestinationFuncAsString, string propertyName, string callingTypeFullName, string callingAssemblyFullName)
        {
            try
            {
                    var assemblyQualifiedName = GetAssemblyQualifiedName(getDestinationFuncAsString, callingTypeFullName, callingAssemblyFullName);
                var methodName = GetMethodName(getDestinationFuncAsString);
                var func = CreateFuncDelegate3(assemblyQualifiedName, methodName);

                return func;
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to convert [{getDestinationFuncAsString}] for property [{propertyName}] to Func.", e);
            }
        }

        private static string GetAssemblyQualifiedName(string funcDescriptor, string callingTypeFullName, string callingAssemblyFullName)
        {
            var split = funcDescriptor.Split('-');

            if (split.Length > 1)
            {
                var assemblyQualifiedName = split[0].Trim();

                return assemblyQualifiedName;
            }

            split = split[0].Split('.');

            if (split.Length == 1)
            {
                var assemblyQualifiedName = $"{callingTypeFullName}, {callingAssemblyFullName}";

                return assemblyQualifiedName;
            }
            else
            {
                var methodName = split[^1].Trim();
                var className = split[^2].Trim();
                var nameSpace = funcDescriptor.Replace($".{methodName}", string.Empty).Replace($".{className}", string.Empty);
                var assemblyQualifiedName = $"{nameSpace}.{className}, {callingAssemblyFullName}";

                return assemblyQualifiedName;
            }
        }

        private static string GetMethodName(string funcDescriptor)
        {
            var split = funcDescriptor.Split('-');

            if (split.Length > 1)
            {
                var methodName = split[1].Trim();

                return methodName;
            }

            split = split[0].Split('.');

            if (split.Length == 1)
            {
                var methodName = funcDescriptor.Trim();

                return methodName;
            }
            else
            {
                var methodName = split[^1].Trim();

                return methodName;
            }
        }

        internal static Action<object, EventArgs> CreateActionDelegate(string assemblyQualifiedName, string methodName)
        {
            var methodInfo = ObjectCreator.GetMethod(assemblyQualifiedName, methodName);
            if (methodInfo.IsStatic)
            {
                var action = (Action<object, EventArgs>)Delegate.CreateDelegate(typeof(Action<object, EventArgs>), methodInfo);

                return action;
            }
            else
            {
                var instantiatedObject = ObjectCreator.CreateInstance(assemblyQualifiedName);
                var action = (Action<object, EventArgs>)Delegate.CreateDelegate(typeof(Action<object, EventArgs>), instantiatedObject, methodInfo);

                return action;
            }
        }

        private static Func<object, string> CreateFuncDelegate(string assemblyQualifiedName, string methodName)
        {
            var methodInfo = ObjectCreator.GetMethod(assemblyQualifiedName, methodName);
            if (methodInfo.IsStatic)
            {
                var func = (Func<object, string>)Delegate.CreateDelegate(typeof(Func<object, string>), methodInfo);

                return func;
            }
            else
            {
                var instantiatedObject = ObjectCreator.CreateInstance(assemblyQualifiedName);
                var func = (Func<object, string>)Delegate.CreateDelegate(typeof(Func<object, string>), instantiatedObject, methodInfo);

                return func;
            }
        }

        private static Func<IControl, bool> CreateFuncDelegate2(string assemblyQualifiedName, string methodName)
        {
            var methodInfo = ObjectCreator.GetMethod(assemblyQualifiedName, methodName);
            if (methodInfo.IsStatic)
            {
                var func = (Func<IControl, bool>)Delegate.CreateDelegate(typeof(Func<IControl, bool>), methodInfo);

                return func;
            }
            else
            {
                var instantiatedObject = ObjectCreator.CreateInstance(assemblyQualifiedName);
                var func = (Func<IControl, bool>)Delegate.CreateDelegate(typeof(Func<IControl, bool>), instantiatedObject, methodInfo);

                return func;
            }
        }

        private static Func<IControl, Rectangle> CreateFuncDelegate3(string assemblyQualifiedName, string methodName)
        {
            var methodInfo = ObjectCreator.GetMethod(assemblyQualifiedName, methodName);
            if (methodInfo.IsStatic)
            {
                var func = (Func<IControl, Rectangle>)Delegate.CreateDelegate(typeof(Func<IControl, Rectangle>), methodInfo);

                return func;
            }
            else
            {
                var instantiatedObject = ObjectCreator.CreateInstance(assemblyQualifiedName);
                var func = (Func<IControl, Rectangle>)Delegate.CreateDelegate(typeof(Func<IControl, Rectangle>), instantiatedObject, methodInfo);

                return func;
            }
        }

        public override string ToString()
        {
            return DebuggerDisplay;
        }

        public string DebuggerDisplay => $"{{Count={_dictionary.Count}}}";
    }
}