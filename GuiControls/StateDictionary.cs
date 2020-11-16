using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Zen.Utilities;
using Zen.Utilities.ExtensionMethods;

namespace Zen.GuiControls
{
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

        public Color? GetAsColor(string propertyName, Color? dflt)
        {
            var valueAsString = GetString(propertyName);
            if (!valueAsString.HasValue()) return dflt;

            var value = TranslateColor(valueAsString, propertyName);

            return value;
        }

        public Func<object, string> GetAsGetTextFunc(string propertyName, string callingTypeFullName, string callingAssemblyFullName)
        {
            var valueAsString = GetString(propertyName);
            if (!valueAsString.HasValue()) return null;

            var value = TranslateGetTextFunc(valueAsString, propertyName, callingTypeFullName, callingAssemblyFullName);

            return value;
        }

        public List<string> GetAsListOfStrings(string propertyName)
        {
            var valueAsString = GetString(propertyName);
            if (!valueAsString.HasValue()) return new List<string>();

            var containsList = new List<string>();
            try
            {
                var cont = valueAsString.RemoveFirstAndLastCharacters();
                var contains = cont.Split(';');
                containsList.AddRange(contains);
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to get contains for [{valueAsString}].", e);
            }

            return containsList;
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
            try
            {
                var firstAndLastCharactersRemoved = getTextFuncAsString.RemoveFirstAndLastCharacters(); // remove single quotes
                var split = firstAndLastCharactersRemoved.Split('-');

                if (split.Length > 1) // if 'Game1.EventHandlers, Game1 - GetTextFunc'
                {
                    var assemblyQualifiedName = split[0].Trim(); // Game1.EventHandlers, Game1
                    var methodName = split[1].Trim(); // GetTextFunc
                    var func = ObjectCreator.CreateFuncDelegate(assemblyQualifiedName, methodName);

                    return func;
                }

                split = split[0].Split('.');

                if (split.Length == 1)
                {
                    var methodName = split[0].Trim();
                    var assemblyQualifiedName = $"{callingTypeFullName}, {callingAssemblyFullName}";
                    var func = ObjectCreator.CreateFuncDelegate(assemblyQualifiedName, methodName);

                    return func;
                }

                // else: 'Game1.EventHandlers.GetTextFunc'
                if (split.Length > 1)
                {
                    var methodName = split[^1].Trim(); // GetTextFunc
                    var className = split[^2].Trim(); // EventHandlers
                    var nameSpace = firstAndLastCharactersRemoved.Replace($".{methodName}", string.Empty).Replace($".{className}", string.Empty); // Game1
                    var assemblyQualifiedName = $"{nameSpace}.{className}, {callingAssemblyFullName}"; // Game1.EventHandlers, Game1
                    var func = ObjectCreator.CreateFuncDelegate(assemblyQualifiedName, methodName);

                    return func;
                }

                throw new Exception($"Failed to convert [{getTextFuncAsString}] for property [{propertyName}] to Func.");
            }
            catch (Exception e)
            {
                throw new Exception($"Failed to convert [{getTextFuncAsString}] for property [{propertyName}] to Func.", e);
            }
        }
    }
}