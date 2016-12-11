public class NetworkData
{
    public string objectID;
    public int gameState;
    public double serverTimeStamp;

    public NetworkData(string objectID, int gameState, double timeStamp)
    {
        this.objectID = objectID;
        this.gameState = gameState;
        this.serverTimeStamp = timeStamp;
    }

}
