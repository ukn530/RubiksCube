using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class CubeModel
{
    // 角の位置（0～7）、角の回転（0～2）
    int[] _cp = new int[8];// corner permutation
    public int[] CP => _cp;
    int[] _co = new int[8]; // corner orientation
    public int[] CO => _co;

    // 辺の位置（0～11）、辺の回転（0,1）
    int[] _ep = new int[12]; // edge permutation
    public int[] EP => _ep;
    int[] _eo = new int[12]; // edge orientation
    public int[] EO => _eo;

    Dictionary<string, CubeModel> _moves = new Dictionary<string, CubeModel>();
    public Dictionary<string, CubeModel> Moves => _moves;
    List<string> _moveNames = new List<string>();
    string[] _faces = new string[] { "U", "D", "L", "R", "F", "B" };
    Dictionary<string, string> _invFace = new Dictionary<string, string>
    {
        { "U", "D" },
        { "D", "U" },
        { "L", "R" },
        { "R", "L" },
        { "F", "B" },
        { "B", "F" }
    };

    List<string> _currentSolution = new List<string>();

    public CubeModel()
    {
        InitializeCube();
        InitializeMoves();
    }

    public CubeModel(int[] newcp, int[] newco, int[] newep, int[] neweo)
    {
        _cp = newcp;
        _co = newco;
        _ep = newep;
        _eo = neweo;
    }

    public CubeModel ApplyMove(CubeModel move)
    {
        int[] newCP = new int[8];
        int[] newCO = new int[8];
        int[] newEP = new int[12];
        int[] newEO = new int[12];

        for (int i = 0; i < 8; i++)
        {
            newCP[i] = _cp[move.CP[i]];
            newCO[i] = (_co[move.CP[i]] + move.CO[i]) % 3;
        }
        for (int i = 0; i < 12; i++)
        {
            newEP[i] = _ep[move.EP[i]];
            newEO[i] = (_eo[move.EP[i]] + move.EO[i]) % 2;
        }

        return new CubeModel(newCP, newCO, newEP, newEO);
        // _cp = newCP;
        // _co = newCO;
        // _ep = newEP;
        // _eo = newEO;
        // return this;
    }

    public void InitializeCube()
    {
        for (int i = 0; i < 8; i++)
        {
            _cp[i] = i;
            _co[i] = 0;
        }

        for (int i = 0; i < _ep.Length; i++)
        {
            _ep[i] = i;
            _eo[i] = 0;
        }
    }

    void InitializeMoves()
    {
        _moves[_faces[0]] = new CubeModel(
                    new int[] { 3, 0, 1, 2, 4, 5, 6, 7 },
                    new int[] { 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[] { 0, 1, 2, 3, 7, 4, 5, 6, 8, 9, 10, 11 },
                    new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
                );
        _moves[_faces[1]] = new CubeModel(
                    new int[] { 0, 1, 2, 3, 5, 6, 7, 4 },
                    new int[] { 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 9, 10, 11, 8 },
                    new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
                );
        _moves[_faces[2]] = new CubeModel(
                    new int[] { 4, 1, 2, 0, 7, 5, 6, 3 },
                    new int[] { 2, 0, 0, 1, 1, 0, 0, 2 },
                    new int[] { 11, 1, 2, 7, 4, 5, 6, 0, 8, 9, 10, 3 },
                    new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
                );
        _moves[_faces[3]] = new CubeModel(
                    new int[] { 0, 2, 6, 3, 4, 1, 5, 7 },
                    new int[] { 0, 1, 2, 0, 0, 2, 1, 0 },
                    new int[] { 0, 5, 9, 3, 4, 2, 6, 7, 8, 1, 10, 11 },
                    new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
                );
        _moves[_faces[4]] = new CubeModel(
                    new int[] { 0, 1, 3, 7, 4, 5, 2, 6 },
                    new int[] { 0, 0, 1, 2, 0, 0, 2, 1 },
                    new int[] { 0, 1, 6, 10, 4, 5, 3, 7, 8, 9, 2, 11 },
                    new int[] { 0, 0, 1, 1, 0, 0, 1, 0, 0, 0, 1, 0 }
                );
        _moves[_faces[5]] = new CubeModel(
                    new int[] { 1, 5, 2, 3, 0, 4, 6, 7 },
                    new int[] { 1, 2, 0, 0, 2, 1, 0, 0 },
                    new int[] { 4, 8, 2, 3, 1, 5, 6, 7, 0, 9, 10, 11 },
                    new int[] { 1, 1, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0 }
                );

        string r180 = "2";
        string r270 = "'";

        foreach (var face in _faces)
        {
            _moveNames.Add(face);
            _moveNames.Add(face + r180);
            _moveNames.Add(face + r270);

            _moves[face + r180] = _moves[face].ApplyMove(_moves[face]);
            _moves[face + r270] = _moves[face].ApplyMove(_moves[face]).ApplyMove(_moves[face]);
        }
    }

    public CubeModel ScrambleToState(string scramble)
    {
        CubeModel scrambledState = new CubeModel(_cp, _co, _ep, _eo);
        foreach (var moveName in scramble.Split(' '))
        {
            var moveState = _moves[moveName];
            scrambledState = scrambledState.ApplyMove(moveState);
        }
        _cp = scrambledState.CP;
        _co = scrambledState.CO;
        _ep = scrambledState.EP;
        _eo = scrambledState.EO;
        return this;
    }

    public bool IsSolved(CubeModel state)
    {
        for (int i = 0; i < 8; i++)
        {
            if (state.CP[i] != i || state.CO[i] != 0)
                return false;
        }
        for (int i = 0; i < 12; i++)
        {
            if (state.EP[i] != i || state.EO[i] != 0)
                return false;
        }
        return true;
    }

    public bool IsMoveAvailable(string prevMove, string move)
    {
        if (string.IsNullOrEmpty(prevMove))
            return true; // 最初の1手はどの操作も可

        string prevFace = prevMove.Substring(0, 1); //U' U2などはUになる
        string moveFace = move.Substring(0, 1);

        if (prevFace == moveFace)
            return false; // 同一面は不可

        if (_invFace[prevFace] == moveFace)
            return string.Compare(prevFace, moveFace) < 0; // 対面のときは、辞書順なら可

        return true;
    }

    public bool DepthLimitedSearch(CubeModel state, int depth)
    {
        if (depth == 0 && IsSolved(state))
        {
            return true;
        }
        else if (depth == 0)
        {
            return false;
        }

        string prevMove = _currentSolution.Count > 0 ? _currentSolution[_currentSolution.Count - 1] : null;
        foreach (var moveName in _moveNames)
        {
            //Debug.Log("moveName: " + moveName);
            //Debug.Log("prevMove: " + prevMove);
            if (!IsMoveAvailable(prevMove, moveName)) //全ての動かし方と前回の動かし方を比較して、同じ面を動かす場合は次のループにスキップ
                continue;

            _currentSolution.Add(moveName);
            //ここまできてない
            if (DepthLimitedSearch(state.ApplyMove(Moves[moveName]), depth - 1))
                return true;
            _currentSolution.RemoveAt(_currentSolution.Count - 1);

            //たとえば3手で解ける場合、1手目でUを試し、2手目Rを試し、3手目でFを試したときに解けたら、その順序を記録しているので答えになる。
            //樹形図を作って上から全て試しているイメージ
        }
        return false;
    }

    public string StartSearch(int maxLength = 20)
    {
        for (int depth = 0; depth < maxLength; depth++)
        {
            Debug.Log($"# Start searching length {depth}");
            if (DepthLimitedSearch(this, depth))
            {
                return string.Join(" ", _currentSolution);//Listをスペース区切りの文字列に変換
            }
        }
        Debug.Log("No solution found");
        return null;
    }
}