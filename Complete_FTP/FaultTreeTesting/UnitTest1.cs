using Microsoft.VisualStudio.TestTools.UnitTesting;
using FaultTreeProject;
using System.Xml;
using System.Collections.Generic;

namespace FaultTreeTesting
{
    [TestClass]
    public class UnitTest1
    {
        // #### XmlNode to Object test #### \\

        [TestMethod]
        public void IsNodeIDCorrect()
        {
            int nodeTestID = 100;
            int pLayer = 100;
            GateNode Node = new GateNode(nodeTestID, pLayer);

            Assert.AreEqual(nodeTestID, Node.mNodeID);
        }

        [TestMethod]
        public void IsEventNodeChildAddedCorrect()
        {
            int ParentNodeID = 100;
            int EventNodeID = 102;
            int pLayer = 100;

            GateNode parentGateNode = new GateNode(ParentNodeID, pLayer);

            // Creates new Xml document
            XmlDocument doc = new XmlDocument();

            // Add parent, children list and the child node element
            XmlNode parentXmlNode = doc.CreateNode(XmlNodeType.Element, "Parent", "");

            XmlNode childrenListXmlNode = doc.CreateNode(XmlNodeType.Element, "Children", "");
            parentXmlNode.AppendChild(childrenListXmlNode);

            XmlNode childXmlNode = doc.CreateNode(XmlNodeType.Element, "Event", "");
            (childXmlNode as XmlElement).SetAttribute("Test", EventNodeID.ToString());
            childrenListXmlNode.AppendChild(childXmlNode);

            // Run add children method
            OrGateNode.AddChildren(parentXmlNode, parentGateNode);

            Assert.AreEqual(parentGateNode.EventChildNodeList[0].mNodeID, EventNodeID);
        }

        [TestMethod]
        public void IsOrGateNodeAddedCorrect()
        {
            int ParentNodeID = 100;
            int OrGateNodeID = 102;
            int pLayer = 100;

            GateNode parentGateNode = new GateNode(ParentNodeID, pLayer);

            // Creates new Xml document
            XmlDocument doc = new XmlDocument();

            // Add parent, children list and the child node element
            XmlNode parentXmlNode = doc.CreateNode(XmlNodeType.Element, "Parent", "");

            XmlNode childrenListXmlNode = doc.CreateNode(XmlNodeType.Element, "Children", "");
            parentXmlNode.AppendChild(childrenListXmlNode);

            XmlNode childXmlNode = doc.CreateNode(XmlNodeType.Element, "Or", "");
            (childXmlNode as XmlElement).SetAttribute("Test", OrGateNodeID.ToString());
            childrenListXmlNode.AppendChild(childXmlNode);

            // Run add children method
            OrGateNode.AddChildren(parentXmlNode, parentGateNode);

            Assert.AreEqual(parentGateNode.GateChildNodeList[0].mNodeID, OrGateNodeID);
        }

        [TestMethod]
        public void IsAndGateNodeAddedCorrect()
        {
            int ParentNodeID = 100;
            int AndGateNodeID = 102;
            int pLayer = 100;

            GateNode parentGateNode = new GateNode(ParentNodeID, pLayer);

            // Creates new Xml document
            XmlDocument doc = new XmlDocument();

            // Add parent, children list and the child node element
            XmlNode parentXmlNode = doc.CreateNode(XmlNodeType.Element, "Parent", "");

            XmlNode childrenListXmlNode = doc.CreateNode(XmlNodeType.Element, "Children", "");
            parentXmlNode.AppendChild(childrenListXmlNode);

            XmlNode childXmlNode = doc.CreateNode(XmlNodeType.Element, "Or", "");
            (childXmlNode as XmlElement).SetAttribute("Test", AndGateNodeID.ToString());
            childrenListXmlNode.AppendChild(childXmlNode);

            // Run add children method
            OrGateNode.AddChildren(parentXmlNode, parentGateNode);

            Assert.AreEqual(parentGateNode.GateChildNodeList[0].mNodeID, AndGateNodeID);
        }






        // #### Cutset Test #### \\

        [TestMethod]
        public void IsCutsetsGeneratedCorrect()
        {
            Assert.Inconclusive();
        }

        [TestMethod]
        public void correctnumberofcutsets()
        {

        }
    }
}
