using System.Xml.Linq;

namespace Blaven
{
    public static class XElementExtensions
    {
        public static string TryGetElementValue(this XElement element, XName name, string defaultValue = "")
        {
            var selectedElement = element.Element(name);
            if (selectedElement == null)
            {
                return defaultValue;
            }

            string value = selectedElement.Value;
            return !string.IsNullOrWhiteSpace(value) ? value : defaultValue;
        }

        public static string TryGetAttributeValue(this XElement element, XName name, string defaultValue = "")
        {
            var selectedAttribute = element.Attribute(name);
            if (selectedAttribute == null)
            {
                return defaultValue;
            }

            string value = selectedAttribute.Value;
            return !string.IsNullOrWhiteSpace(value) ? value : defaultValue;
        }

        public static bool GetElementExists(this XElement element, XName name)
        {
            var selectedElement = element.Element(name);
            return selectedElement != null;
        }
    }
}
