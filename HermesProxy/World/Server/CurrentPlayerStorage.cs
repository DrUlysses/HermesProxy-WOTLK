namespace HermesProxy.World.Server;

public class CurrentPlayerStorage
{
	private readonly GlobalSessionData _globalSession;

	public CompletedQuestTracker CompletedQuests { get; private set; }

	public PlayerSettings Settings { get; private set; }

	public CurrentPlayerStorage(GlobalSessionData globalSession)
	{
		_globalSession = globalSession;
	}

	public void LoadCurrentPlayer()
	{
		CompletedQuests = new CompletedQuestTracker(_globalSession);
		Settings = new PlayerSettings(_globalSession);
		CompletedQuests.Reload();
		Settings.Reload();
	}
}
