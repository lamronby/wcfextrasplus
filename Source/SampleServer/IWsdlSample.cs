using System.Runtime.Serialization;
using System.ServiceModel;
using WCFExtrasPlus.Wsdl.Documentation;

namespace Sample
{
    /// <summary>
    /// This is a sample service contract that demonstrates how xml comments
    /// can be placed anywhere in code are rendered to the result WSDL.
    /// These comments will also be imported to the client generated proxy if the proxy configured correctly
    /// </summary>
	[XmlComments]
    [ServiceContract]
    public interface IWsdlSample
    {
        /// <summary>
        /// This is a simple operation without input paramaters.
        /// </summary>
        /// <returns>The operations returns a string</returns>
        /// <seealso cref="Unresolved.Reference"/>
        [OperationContract]
        string Operation1();


        /// <summary>
        /// Receives 2 parameters, both are data contracts
        /// </summary>
        /// <param name="data">Regular DataContract parameter</param>
        /// <param name="innerData">This parameter is from a type declared as an inner class inside a DataContract</param>
        [OperationContract]
        void Operation2(InheritedContract data, DataContractSample.DataContractSampleInner innerData);

        /// This text is placed outside of any tag. It will not be rendered when using the Portable format
        /// <summary>
        /// Operation 3 - This is a <b>test</b> for very long xml comments. The rest of this line is just garbage: Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Donec magna odio, imperdiet in, facilisis vitae, congue vel, urna. Aliquam ullamcorper. Integer libero turpis, tempor quis, interdum ac, interdum sed, lorem. Nam ipsum erat, interdum sed, pellentesque sed, faucibus sed, risus. In commodo ipsum eget neque. Suspendisse ut nisl sit amet odio semper luctus. Maecenas sit amet nisl. Sed ac ligula eu nunc lacinia pulvinar. Phasellus auctor aliquam diam. Curabitur at nibh id felis mattis ornare. Quisque ac purus non odio aliquet condimentum. Curabitur gravida nisi a odio. In iaculis pede vitae dui. Nunc a mi. Curabitur elementum ligula in odio. Nunc accumsan egestas odio
        /// </summary>
        /// <remarks>
        ///     This is a section of remarks. 
        /// This information will go into the HTML documentation 
        /// Remarks are not rendered when using Portable
        /// </remarks>
        /// seealso references are not renderd when using Portable
        /// <param name="str">a Nullable parameter</param>
        /// <param name="param1">this is
        /// a 
        /// multiline 
        /// commnet
        /// for param2</param>
        /// <param name="param2">a parameter passed by reference</param>
        /// <returns>this method returns a string</returns>
        /// <custom>This is a custom XML comment tag</custom>
        [OperationContract]
        string Operation3(Struct? str, out int param1, ref long param2);

        /// <summary>
        /// This operation uses an enum as a parameter.
        /// </summary>
        /// <param name="param1">A "regular" enum. This is an enum not marked with the [DataContract] attribure</param>
        /// <seealso cref="Operation3"/>
        [OperationContract]
        void Operation4(RegularEnum param1);
    }

    /// <summary>
    /// A basic class marked with [DataContract]
    /// </summary>
    [DataContract]
    public class DataContractSample
    {
        /// <summary>
        /// An class declared as an inner 
        /// </summary>
        [DataContract]
        public class DataContractSampleInner
        {
            /// <summary>
            /// A member field of an inner class
            /// </summary>
            [DataMember]
            public string Member1 { get; set; }

            /// <summary>
            /// This summary references <see cref="Member1">Member1</see>
            /// </summary>
            [DataMember]
            public string Member2 { get; set; }
        }

        /// <summary>
        /// A simple string member
        /// </summary>
        [DataMember]
        public string Member1 { get; set; }

        /// <summary>
        /// An enum Field (Not a property)
        /// </summary>
        [DataMember]
        public ErrorCodes ErrorCodeField;

        /// <summary>
        /// A member who's name was changed using the DataMember attribute.
        /// Also note that the new member name has special chars.
        /// This summary references <see cref="Member1">Member1</see>
        /// </summary>
        [DataMember(Name = "@WeirdName")]
        public int Member2 { get; set; }

        /// <summary>
        /// A member with a name that collides with the ExtensionData member 
        /// inherited from IExtensibleDataObject
        /// </summary>
        [DataMember]
        public string ExtensionData { get; set; }

    }

    /// <summary>
    /// An enum marked with a DataContract attribute
    /// </summary>
    [DataContract]
    public enum ErrorCodes
    {
        /// <summary>
        /// Unknown error has occoured
        /// </summary>
        [EnumMember]
        General = -1,

        /// <summary>
        /// An enum member who's name was changed using the EnumMember attribute.
        /// Also note that the new member name has special chars.
        /// </summary>
        /// <seealso cref="IWsdlSample.Operation2"/>
        [EnumMember(Value = "@FileNotFound")]
        FileNotFound = 1,

        /// <summary>
        /// Out of memory see <see cref="IWsdlSample.Operation1"/> for more details
        /// </summary>
        [EnumMember]
        OutOfMemory = 2,
    }

    /// <summary>
    /// This is a "regular" enum without a DataContract attribute.
    /// </summary>
    public enum RegularEnum
    {
        /// <summary>
        /// First value documentation
        /// </summary>
        /// see <see cref="ErrorCodes"/> and <see cref="ErrorCodes.FileNotFound"/>
        FirstValue = 1,
        SecondValue = 2,
    }

    /// <summary>
    /// Structs can also be documented
    /// </summary>
    [DataContract]
    public struct Struct
    {
        /// <summary>
        /// And members of structs
        /// </summary>
        [DataMember]
        public string str;
    }

	/// <summary>
	/// A data contract that inherits from DataContractSample.
	/// </summary>
	[DataContract]
	public class InheritedContract : DataContractSample
	{
		/// <summary>
		/// Gets or sets the property.
		/// </summary>
		/// <value>The property.</value>
		[DataMember]
		public string Property
		{
			get;
			set;
		}
	}
}

