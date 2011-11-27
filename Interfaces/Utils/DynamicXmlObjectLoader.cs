using System;
using System.Xml;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using System.Diagnostics;
using System.Runtime.Remoting;


namespace B4F.TotalGiro.Utils
{

    /// <summary>
    /// This class represents an exception that may be encountered when
    /// attempting to load a type from an XML configuration file.</summary>
    public class DynamicXmlObjectLoaderException : Exception
    {
    public DynamicXmlObjectLoaderException(string message, Exception innerException)
      : base(message, innerException) { }

    public DynamicXmlObjectLoaderException(string message)
      : base(message) { }

    public DynamicXmlObjectLoaderException()
      : base() { }
    }

    /// <summary>
    /// This class parses an XML file and dynamically loads an object.</summary>
    /// <remarks>
    /// The object type along with its properties and fields are specified in the
    /// XML file. The object is dynamically instantiated from the type name, so
    /// an assemblies can be added with new types without recompiling. The
    /// values specified can also be objects which are recursively loaded...
    /// See the example jobs.config file in the AgentTestApp and the readme.txt
    /// file in this project (AgentClasses).</remarks>
    public class DynamicXmlObjectLoader
    {
        /// <summary>
        /// This is the primary load method.</summary>
        public object Load(string url)
        {
            object loadedObject = null;

          // Start reading the XML from the URL
          XmlTextReader xmlReader = new XmlTextReader(url);
          {
            while ( xmlReader.Read() ) 
              // get the current node's (namespace) name
              if ( xmlReader.NodeType == XmlNodeType.Element )
                loadedObject = LoadXmlObject(xmlReader);
          }

          xmlReader.Close();

          return loadedObject;
        }

        /// <summary>
        /// Loads an object from the current XML element node.</summary>
        /// <remarks>
        /// This should be called when the xmlReader is currently positioned at an
        /// element node. The object is loaded and dynamically instantiated. The
        /// object type is determined from the 'type' attribute, which must be
        /// specified and must be the first attribute. Additional attributes are
        /// loaded into fields or properties of the object - an attempt is made to
        /// convert the text to the correct target type - see SetMember.
        /// Sub-elements within this element are also considered to be properties
        /// or fields but are first (recursively) loaded as new objects.
        /// If a sub-elements refers to a property or field that derives from
        /// CollectionBase, then the Add method is used to add the object to
        /// the collection.</remarks>
        private object LoadXmlObject(XmlReader xmlReader) 
        {
          object newObject = null;
          string typeName = null;
          bool hasSubElements = !xmlReader.IsEmptyElement;

          // process attributes
          while (xmlReader.MoveToNextAttribute())
            // is this the 'type' attribute that specifed the type of the object?
            if ( xmlReader.Name == "type" ) 
            {
              // are we expecting the type attribute now?
              if ( newObject == null )
                // if the object is null, then we expect the type attribute
                typeName = xmlReader.Value;
              else
                // if the object was instantiated, then we have already found other attributes
                throw( new DynamicXmlObjectLoaderException("The 'type' attribute must be the first attribute specified.") );
            }
            else 
            {
              // now we are processing other attributes (not the 'type' attribute)
              // e.g., this attribute is a property or field

              // has the object been instantiated yet? (this may be the first attribute after 'type')
              if ( newObject == null ) 
                newObject = InstantiateObject(typeName);

              // set the object's specified property or field to the specified value
              SetMember(newObject, xmlReader.Name, xmlReader.Value);
            }

          // has the object been instantiated yet? (there may have been no additional attributes)
          if ( newObject == null ) 
            newObject = InstantiateObject(typeName);

          // process elements
          if ( hasSubElements )
            while ( xmlReader.Read() ) 
            {
              if ( xmlReader.NodeType == XmlNodeType.EndElement )
                break;
              else if (xmlReader.NodeType == XmlNodeType.Element) 
              {
                string elementName = xmlReader.Name;
                object elementObject = LoadXmlObject(xmlReader);

                // set the object's specified property or field to the specified value
                SetMember(newObject, elementName, elementObject);
              }
            }

          return newObject;
        }

        /// <summary>
        /// Instantiates an object of the specified type. This is called by LoadXmlObject.</summary>
        private object InstantiateObject(string typeName) 
        {
          // first, make sure that we know what the object type is
          if ( typeName == null )
            throw( new DynamicXmlObjectLoaderException("The 'type' attribute must be the first attribute specified.") );

          // get the Type of the object to instantiate
          Type type = Type.GetType(typeName);
          if ( type == null ) 
            throw( new DynamicXmlObjectLoaderException("Type.GetType failed for type: " + typeName) );

          // return a new instance of the type
          return Activator.CreateInstance( type );
        }

        /// <summary>
        /// Convert a string into the specified target type.
        /// Special handling has been implemented for TimeSpan and Enum types, so that
        /// Enums may be specified by name. Additional special cases could be added here.</summary>
        private object ConvertStringValueToTargetType(Type targetType, string stringValue) 
        {
          if ( targetType == typeof(TimeSpan) )
            // If its a TimeSpan, then parse it
            return TimeSpan.Parse(stringValue);
          else if ( targetType.IsEnum )
            // If its an Enum, then parse it
            return Enum.Parse(targetType, stringValue, true);
          else 
            // Otherwise try a generic conversion
            return Convert.ChangeType(stringValue, targetType);
        }

        /// <summary>
        /// Sets the value of a specific member of the instance object.</summary>
        /// <remarks>
        /// Uses reflection to examine the instance. If the member (property or field)
        /// is an array, the value is assumed to be a comma delimited list of values which
        /// are parsed.
        /// If the member is derived from CollectionBase, then the Add method is called
        /// in order to add the value.</remarks> 
        private void SetMember(object instance, string memberName, object memberValue) 
        {
          // see if there is a property with the specified name
          PropertyInfo propertyInfo = instance.GetType().GetProperty(memberName);
          // see if there is a field with the specified name
          FieldInfo fieldInfo = instance.GetType().GetField(memberName);

          // did we find anything?
          if ( (propertyInfo == null) && (fieldInfo == null) )
            throw(new DynamicXmlObjectLoaderException(string.Format("The property {0} is invalid for {1}.", memberName, instance.GetType())));

          // determine the target type of property or field
          System.Type targetType = (propertyInfo != null) ? propertyInfo.PropertyType : ((fieldInfo != null) ? fieldInfo.FieldType : null);

          // do we need to process the memberValue? (e.g., if it is a string)
          if ( memberValue is string ) 
          {
            // convert the attribute value (a string) to the type required by the property or method
            if (!targetType.IsArray)
              memberValue = ConvertStringValueToTargetType(targetType, (string) memberValue);
            else
            {
              // the type is an array - assume that the attribute value is a comma delimited list of values
              Type targetElementType = targetType.GetElementType();
              ArrayList itemsArrayList = new ArrayList();

              // convert each value to the element type for the array and copy them into an array list
              foreach(string item in ((string) memberValue).Split(new char[] {','}))
                itemsArrayList.Add( ConvertStringValueToTargetType(targetElementType, item) );
            
              // convert the array list into an array
              memberValue = Array.CreateInstance(targetType.GetElementType(), itemsArrayList.Count);
              itemsArrayList.CopyTo((System.Array) memberValue);
            }
          }

          //is the target type a Collection that we can add to?
          if ( targetType.IsSubclassOf( typeof(CollectionBase) ) ) 
          {
            object targetInstance = (propertyInfo != null) ? propertyInfo.GetValue(instance, new object [] {})
              : fieldInfo.GetValue(instance);
            MethodInfo addMethodInfo = targetInstance.GetType().GetMethod("Add");
            addMethodInfo.Invoke(targetInstance, new object [] { memberValue });
          }
          else 
          {
            // now that we have the final mamberValue, set the property or field
            if ( propertyInfo != null )
              propertyInfo.SetValue(instance, memberValue, null);
            else if ( fieldInfo != null )
              fieldInfo.SetValue(instance, memberValue);
          }
        }
    }
}
