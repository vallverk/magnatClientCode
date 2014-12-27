[System.Serializable]
public struct StartedGameFieldData
{
	/// <summary>
	/// Эффект поля
	/// </summary>
	public int E;
	/// <summary>
	/// Владелец
	/// </summary>
	public int O;
	/// <summary>
	/// Ранг монополии
	/// </summary>
	public int R;
	/// <summary>
	/// Заложено? 0 - нет, 1 - да
	/// </summary>
	public int L;
}

[System.Serializable]
public class StartedGameData
{
	/// <summary>
	/// Информация о игре
	/// </summary>
	public GameInfo GI; 
	/// <summary>
	/// Время, когда стартовала игра
	/// </summary>
	public double STT;
	/// <summary>
	/// Время, когда были сняты данные о игре
	/// </summary>
	public double T;
	/// <summary>
	/// Информация о игроках
	/// </summary>
	public Player[] Ps;
	/// <summary>
	/// ID текущего игрока
	/// </summary>
	public int CID;
	/// <summary>
	/// Время старта таймера
	/// </summary>
	public double TST;
	/// <summary>
	/// Инфа о полях
	/// </summary>
	public StartedGameFieldData[] F;
}
