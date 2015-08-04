namespace Sinch.Callback.Response.Internal
{
    internal class ConnectConferenceSvamletResponse : SvamletResponse, IConnectConferenceSvamletResponse
    {
        public IConnectConferenceSvamletResponse WithMusicOnHold(string moh)
        {
            Model.Action.Moh = moh;
            return this;
        }

        public IConnectConferenceSvamletResponse WithRecording()
        {
            Model.Action.Record = true;
            return this;
        }

        public IConnectConferenceSvamletResponse WithoutRecording()
        {
            Model.Action.Record = false;
            return this;
        }
    }
}