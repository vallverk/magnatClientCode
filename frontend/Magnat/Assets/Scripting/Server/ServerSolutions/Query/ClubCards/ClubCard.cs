public class ClubCard
{
	public string _id;
	public string Name;
	public string Description;
	public string Lavel;
	public string Effect;
	public string term;
	public string price;
	public string image;
	public string status;

    public override string ToString()
    {
        return string.Format("_id = {0} \r\n Name = {1} \r\n Description = {2} \r\n Lavel = {3} \r\n Effect = {4} \r\n term = {5} \r\n price = {6} \r\n image = {7} \r\n status = {8} \r\n", _id, Name, Description, Lavel, Effect, term, price, image, status);
    }
}
