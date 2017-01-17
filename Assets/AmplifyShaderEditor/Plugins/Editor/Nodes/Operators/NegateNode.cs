using System;
using UnityEngine;

namespace AmplifyShaderEditor
{
    [Serializable]
    [NodeAttributes( "Negate", "Operators", "Negate or invert any input" )]
    public sealed class NegateNode : ParentNode
    {
        protected override void CommonInit( int uniqueId )
        {
            base.CommonInit( uniqueId );
            AddInputPort( WirePortDataType.OBJECT, false, string.Empty );
            AddOutputPort( WirePortDataType.OBJECT, Constants.EmptyPortValue );
        }

        public override void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
        {
            base.OnInputPortConnected( portId, otherNodeId, otherPortId, activateNode );
            m_inputPorts[ 0 ].MatchPortToConnection();
            m_outputPorts[ 0 ].ChangeType( m_inputPorts[ 0 ].DataType, false );
        }

        public override void OnConnectedOutputNodeChanges( int outputPortId, int otherNodeId, int otherPortId, string name, WirePortDataType type )
        {
            base.OnConnectedOutputNodeChanges( outputPortId, otherNodeId, otherPortId, name, type );
            m_inputPorts[ 0 ].MatchPortToConnection();
            m_outputPorts[ 0 ].ChangeType( m_inputPorts[ 0 ].DataType, false );
        }

        public override string GenerateShaderForOutput( int outputId, WirePortDataType inputPortType, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
        {
            base.GenerateShaderForOutput( outputId, inputPortType, ref dataCollector, ignoreLocalvar );
            string result = m_inputPorts[ 0 ].GenerateShaderForOutput( ref dataCollector, inputPortType, ignoreLocalvar );

            if ( result.StartsWith( "-" ) )
            {
                return result.Remove( 0, 1 );
            }
            else
            {
                return "-" + result;
            }
        }
    }
}
