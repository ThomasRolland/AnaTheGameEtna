   /* 
    Licensed under the Apache License, Version 2.0
    
    http://www.apache.org/licenses/LICENSE-2.0
    */
    using System;
    using System.Xml.Serialization;
    using System.Collections.Generic;
    namespace Xml2CSharp
    {
	/* NOTES */
    	[XmlRoot(ElementName="Note")]
    	public class Note {
    		[XmlElement(ElementName="string")]
    		public string String { get; set; }
    		[XmlAttribute(AttributeName="name")]
    		public string Name { get; set; }
    	}

    	[XmlRoot(ElementName="Notes")]
    	public class Notes {
    		[XmlElement(ElementName="Note")]
    		public List<Note> Note { get; set; }
    	}
	/* TRANSCRIPTES */
    	[XmlRoot(ElementName="string")]
    	public class String {
    		[XmlAttribute(AttributeName="category")]
    		public string Category { get; set; }
    		[XmlText]
    		public string Text { get; set; }
    	}

    	[XmlRoot(ElementName="Group")]
    	public class Group {
    		[XmlElement(ElementName="string")]
    		public List<String> String { get; set; }
    		[XmlAttribute(AttributeName="name")]
    		public string Name { get; set; }
    	}

    	[XmlRoot(ElementName="Transcripts")]
    	public class Transcripts {
    		[XmlElement(ElementName="Group")]
    		public List<Group> Group { get; set; }
    	}


    	
	/* GAME TEXTES INGAME */
    	[XmlRoot(ElementName="GameOver")]
    	public class GameOver {
    		[XmlElement(ElementName="string")]
    		public List<string> String { get; set; }
    	}

    	[XmlRoot(ElementName="InGameIsLosing")]
    	public class InGameIsLosing {
    		[XmlElement(ElementName="string")]
    		public List<string> String { get; set; }
    	}

    	[XmlRoot(ElementName="InGameWinning")]
    	public class InGameWinning {
    		[XmlElement(ElementName="string")]
    		public List<string> String { get; set; }
    	}

    	[XmlRoot(ElementName="HasLoose")]
    	public class HasLoose {
    		[XmlElement(ElementName="string")]
    		public List<string> String { get; set; }
    	}

    	[XmlRoot(ElementName="HasDraw")]
    	public class HasDraw {
    		[XmlElement(ElementName="string")]
    		public List<string> String { get; set; }
    	}

    	[XmlRoot(ElementName="Texts")]
    	public class Texts {
    		[XmlElement(ElementName="GameOver")]
    		public GameOver GameOver { get; set; }
    		[XmlElement(ElementName="InGameIsLosing")]
    		public InGameIsLosing InGameIsLosing { get; set; }
    		[XmlElement(ElementName="InGameWinning")]
    		public InGameWinning InGameWinning { get; set; }
    		[XmlElement(ElementName="HasLoose")]
    		public HasLoose HasLoose { get; set; }
    		[XmlElement(ElementName="HasDraw")]
    		public HasDraw HasDraw { get; set; }
    	}
    }
