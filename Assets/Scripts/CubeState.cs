
public class CubeState
{
    // 角の位置（0～7）、角の回転（0～2）
    int[] _cp = new int[8];// corner permutation
    public int[] CP { get => _cp; set => _cp = value; }
    int[] _co = new int[8]; // corner orientation
    public int[] CO { get => _co; set => _co = value; }

    // 辺の位置（0～11）、辺の回転（0,1）
    int[] _ep = new int[12]; // edge permutation
    public int[] EP { get => _ep; set => _ep = value; }
    int[] _eo = new int[12]; // edge orientation
    public int[] EO { get => _eo; set => _eo = value; }

    public CubeState()
    {
        for (int i = 0; i < 8; i++)
        {
            _cp[i] = i;
            _co[i] = 0;
        }

        for (int i = 0; i < 12; i++)
        {
            _ep[i] = i;
            _eo[i] = 0;
        }
    }

    public CubeState(int[] newcp, int[] newco, int[] newep, int[] neweo)
    {
        _cp = newcp;
        _co = newco;
        _ep = newep;
        _eo = neweo;
    }



}
