using System.Collections.Generic;
using UnityEngine;

public class CubeModel
{
    Dictionary<string, CubeState> _moves = new Dictionary<string, CubeState>();
    public Dictionary<string, CubeState> Moves => _moves;
    List<string> _moveNames = new List<string>();
    readonly List<string> _moveNamesPh2 = new List<string> { "U", "U2", "U'", "D", "D2", "D'", "L2", "R2", "F2", "B2" };
    string[] _faces = new string[] { "U", "D", "L", "R", "F", "B" };
    List<string> _currentSolution = new List<string>();
    Dictionary<string, string> _invFace = new Dictionary<string, string>
    {
        { "U", "D" },
        { "D", "U" },
        { "L", "R" },
        { "R", "L" },
        { "F", "B" },
        { "B", "F" }
    };

    // Constants for cube properties
    const int NumCorners = 8;
    const int NumEdges = 12;
    const int NumCo = 2187;
    const int NumEo = 2048;
    const int NumECombinations = 495; // 12 choose 4, for example
    const int NumCp = 40320; // 8!
    const int NumUdEp = 40320; // 12 choose 4
    const int NumEEp = 792; // 12 choose 5

    public CubeModel()
    {
        InitializeMoves();
    }

    public CubeState ApplyMove(CubeState state, CubeState move)
    {
        int[] newCP = new int[8];
        int[] newCO = new int[8];
        int[] newEP = new int[12];
        int[] newEO = new int[12];

        for (int i = 0; i < 8; i++)
        {
            newCP[i] = state.CP[move.CP[i]];
            newCO[i] = (state.CO[move.CP[i]] + move.CO[i]) % 3;
        }
        for (int i = 0; i < 12; i++)
        {
            newEP[i] = state.EP[move.EP[i]];
            newEO[i] = (state.EO[move.EP[i]] + move.EO[i]) % 2;
        }

        return new CubeState(newCP, newCO, newEP, newEO);//stateを変更すると、元のstateが変わってしまうので、newCubeStateを返す

    }

    void InitializeMoves()
    {
        _moves[_faces[0]] = new CubeState(
                    new int[] { 3, 0, 1, 2, 4, 5, 6, 7 },
                    new int[] { 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[] { 0, 1, 2, 3, 7, 4, 5, 6, 8, 9, 10, 11 },
                    new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
                );
        _moves[_faces[1]] = new CubeState(
                    new int[] { 0, 1, 2, 3, 5, 6, 7, 4 },
                    new int[] { 0, 0, 0, 0, 0, 0, 0, 0 },
                    new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 9, 10, 11, 8 },
                    new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
                );
        _moves[_faces[2]] = new CubeState(
                    new int[] { 4, 1, 2, 0, 7, 5, 6, 3 },
                    new int[] { 2, 0, 0, 1, 1, 0, 0, 2 },
                    new int[] { 11, 1, 2, 7, 4, 5, 6, 0, 8, 9, 10, 3 },
                    new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
                );
        _moves[_faces[3]] = new CubeState(
                    new int[] { 0, 2, 6, 3, 4, 1, 5, 7 },
                    new int[] { 0, 1, 2, 0, 0, 2, 1, 0 },
                    new int[] { 0, 5, 9, 3, 4, 2, 6, 7, 8, 1, 10, 11 },
                    new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
                );
        _moves[_faces[4]] = new CubeState(
                    new int[] { 0, 1, 3, 7, 4, 5, 2, 6 },
                    new int[] { 0, 0, 1, 2, 0, 0, 2, 1 },
                    new int[] { 0, 1, 6, 10, 4, 5, 3, 7, 8, 9, 2, 11 },
                    new int[] { 0, 0, 1, 1, 0, 0, 1, 0, 0, 0, 1, 0 }
                );
        _moves[_faces[5]] = new CubeState(
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

            _moves[face + r180] = ApplyMove(_moves[face], _moves[face]);
            _moves[face + r270] = ApplyMove(_moves[face + r180], _moves[face]);
        }
    }

    public CubeState ScrambleToState(CubeState state, string scramble)
    {
        CubeState scrambledState = new CubeState(state.CP, state.CO, state.EP, state.EO);
        foreach (var moveName in scramble.Split(' '))
        {
            var moveState = _moves[moveName];
            scrambledState = ApplyMove(scrambledState, moveState);
        }
        return scrambledState;
    }

    public int CoToIndex(int[] co)
    {
        int index = 0;
        for (int i = 0; i < co.Length - 1; i++)
        {
            index *= 3;
            index += co[i];
        }
        return index;
    }

    public int[] IndexToCo(int index)
    {
        int[] co = new int[8];
        int sumCo = 0;
        for (int i = 6; i >= 0; i--)
        {
            co[i] = index % 3;
            index /= 3;
            sumCo += co[i];
        }
        co[7] = (3 - sumCo % 3) % 3;
        return co;
    }

    public int EoToIndex(int[] eo)
    {
        int index = 0;
        for (int i = 0; i < eo.Length - 1; i++)
        {
            index *= 2;
            index += eo[i];
        }
        return index;
    }

    public int[] IndexToEo(int index)
    {
        int[] eo = new int[12];
        int sumEo = 0;
        for (int i = 10; i >= 0; i--)
        {
            eo[i] = index % 2;
            index /= 2;
            sumEo += eo[i];
        }
        eo[11] = (2 - sumEo % 2) % 2;
        return eo;
    }

    //nCrの計算
    int CalcCombination(int n, int r)
    {
        int ret = 1;
        for (int i = 0; i < r; i++)
            ret *= n - i;
        for (int i = 0; i < r; i++)
            ret /= r - i;
        return ret;
    }

    //難しい。12こから4つ選ぶ組み合わせの種類にindexを当てはめる
    //[1,1,1,1,0,0,0,0,0,0,0,0]のような配列を与えると、indexは0になる
    //[1,1,1,0,1,0,0,0,0,0,0,0]のような配列を与えると、indexは1になる
    public int ECombinationToIndex(int[] comb)
    {
        int index = 0;
        int r = 4;
        for (int i = 11; i >= 0; i--)
        {
            if (comb[i] == 1)
            {
                index += CalcCombination(i, r);
                r--;
            }
        }
        return index;
    }

    public int[] IndexToECombination(int index)
    {
        int[] combination = new int[12];
        int r = 4;
        for (int i = 11; i >= 0; i--)
        {
            int c = CalcCombination(i, r);
            if (index >= c)
            {
                combination[i] = 1;
                index -= c;
                r--;
            }
            else
            {
                combination[i] = 0;
            }
        }
        return combination;
    }

    public int CpToIndex(int[] cp)
    {
        int index = 0;
        for (int i = 0; i < 8; i++)
        {
            index *= (8 - i);
            for (int j = i + 1; j < 8; j++)
            {
                if (cp[i] > cp[j])
                    index++;
            }
        }
        return index;
    }

    public int[] IndexToCp(int index)
    {
        int[] cp = new int[8];
        for (int i = 6; i >= 0; i--)
        {
            cp[i] = index % (8 - i);
            index /= (8 - i);
            for (int j = i + 1; j < 8; j++)
            {
                if (cp[j] >= cp[i])
                    cp[j]++;
            }
        }
        return cp;
    }

    public int UdEpToIndex(int[] ep)
    {
        int index = 0;
        for (int i = 0; i < 8; i++)
        {
            index *= (8 - i);
            for (int j = i + 1; j < 8; j++)
            {
                if (ep[i] > ep[j])
                    index++;
            }
        }
        return index;
    }

    public int[] IndexToUdEp(int index)
    {
        int[] ep = new int[8];
        for (int i = 6; i >= 0; i--)
        {
            ep[i] = index % (8 - i);
            index /= (8 - i);
            for (int j = i + 1; j < 8; j++)
            {
                if (ep[j] >= ep[i])
                    ep[j]++;
            }
        }
        return ep;
    }

    public int EEpToIndex(int[] eep)
    {
        int index = 0;
        for (int i = 0; i < 4; i++)
        {
            index *= (4 - i);
            for (int j = i + 1; j < 4; j++)
            {
                if (eep[i] > eep[j])
                    index++;
            }
        }
        return index;
    }

    public int[] IndexToEEp(int index)
    {
        int[] eep = new int[4];
        for (int i = 2; i >= 0; i--)
        {
            eep[i] = index % (4 - i);
            index /= (4 - i);
            for (int j = i + 1; j < 4; j++)
            {
                if (eep[j] >= eep[i])
                    eep[j]++;
            }
        }
        return eep;
    }

    //指定のCOのIndexに対して動かした時のCOのIndexを返す
    //例えば[10,1]だと、Indexが10のCOをIndex1のmoveNameで動かした時のCOのIndexが入る
    public int[,] BuildCoMoveTable()
    {
        int[,] coMoveTable = new int[NumCo, _moveNames.Count];
        for (int i = 0; i < NumCo; i++)
        {
            var state = new CubeState(
                new int[8],           // CP: all zeros
                IndexToCo(i),         // CO: from index
                new int[12],          // EP: all zeros
                new int[12]           // EO: all zeros
            );
            for (int iMove = 0; iMove < _moveNames.Count; iMove++)
            {
                var moveName = _moveNames[iMove];
                var newState = ApplyMove(state, _moves[moveName]);
                coMoveTable[i, iMove] = CoToIndex(newState.CO);
            }
        }
        return coMoveTable;
    }

    public int[,] BuildEoMoveTable()
    {
        int[,] eoMoveTable = new int[NumEo, _moveNames.Count];
        for (int i = 0; i < NumEo; i++)
        {
            var state = new CubeState(
                new int[8],           // CP: all zeros
                new int[8],           // CO: all zeros
                new int[12],          // EP: all zeros
                IndexToEo(i)          // EO: from index
            );
            for (int iMove = 0; iMove < _moveNames.Count; iMove++)
            {
                var moveName = _moveNames[iMove];
                var newState = ApplyMove(state, _moves[moveName]);
                eoMoveTable[i, iMove] = EoToIndex(newState.EO);
            }
        }
        return eoMoveTable;
    }

    public int[,] BuildECombinationMoveTable()
    {
        int[,] eCombinationTable = new int[NumECombinations, _moveNames.Count];
        for (int i = 0; i < NumECombinations; i++)
        {
            var state = new CubeState(
                new int[8],                // CP: all zeros
                new int[8],                // CO: all zeros
                IndexToECombination(i),    // EP: from index
                new int[12]                // EO: all zeros
            );
            for (int iMove = 0; iMove < _moveNames.Count; iMove++)
            {
                var moveName = _moveNames[iMove];
                var newState = ApplyMove(state, _moves[moveName]);
                eCombinationTable[i, iMove] = ECombinationToIndex(newState.EP);
            }
        }
        return eCombinationTable;
    }

    public int[,] BuildCpMoveTablePh2()
    {
        int[,] cpMoveTable = new int[NumCp, _moveNamesPh2.Count];
        for (int i = 0; i < NumCp; i++)
        {
            var state = new CubeState(
                IndexToCp(i),
                new int[8],
                new int[12],
                new int[12]
            );
            for (int iMove = 0; iMove < _moveNamesPh2.Count; iMove++)
            {
                var moveName = _moveNamesPh2[iMove];
                var newState = ApplyMove(state, _moves[moveName]);
                cpMoveTable[i, iMove] = CpToIndex(newState.CP);
            }
        }
        return cpMoveTable;
    }

    public int[,] BuildUdEpMoveTablePh2()
    {
        int[,] udEpMoveTable = new int[NumUdEp, _moveNamesPh2.Count];
        for (int i = 0; i < NumUdEp; i++)
        {
            var ep = new int[12];
            var udEp = IndexToUdEp(i);
            for (int j = 0; j < 8; j++)
                ep[j + 4] = udEp[j];

            var state = new CubeState(
                new int[8],    // CP: all zeros
                new int[8],    // CO: all zeros
                ep,            // EP: [0,0,0,0,udEp...]
                new int[12]    // EO: all zeros
            );

            for (int iMove = 0; iMove < _moveNamesPh2.Count; iMove++)
            {
                var moveName = _moveNamesPh2[iMove];
                var newState = ApplyMove(state, _moves[moveName]);
                // Only use ep[4..12] for UdEp index
                int[] newUdEp = new int[8];
                for (int j = 0; j < 8; j++)
                    newUdEp[j] = newState.EP[j + 4];
                udEpMoveTable[i, iMove] = UdEpToIndex(newUdEp);
            }
        }
        return udEpMoveTable;
    }



    public int[,] BuildEEpMoveTablePh2()
    {
        int[,] eEpMoveTable = new int[NumEEp, _moveNamesPh2.Count];
        for (int i = 0; i < NumEEp; i++)
        {
            var eep = IndexToEEp(i);
            var ep = new int[12];
            for (int j = 0; j < 4; j++)
                ep[j] = eep[j];
            // ep[4..11] remain 0

            var state = new CubeState(
                new int[8],    // CP: all zeros
                new int[8],    // CO: all zeros
                ep,            // EP: eep[0..3] + 0s
                new int[12]    // EO: all zeros
            );

            for (int iMove = 0; iMove < _moveNamesPh2.Count; iMove++)
            {
                var moveName = _moveNamesPh2[iMove];
                var newState = ApplyMove(state, _moves[moveName]);
                int[] newEEp = new int[4];
                for (int j = 0; j < 4; j++)
                    newEEp[j] = newState.EP[j];
                eEpMoveTable[i, iMove] = EEpToIndex(newEEp);
            }
        }
        return eEpMoveTable;
    }

    public bool IsSolved(CubeState state)
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

    int CountSolvedCorners(CubeState state)
    {
        int count = 0;
        for (int i = 0; i < 8; i++)
        {
            if (state.CP[i] == i && state.CO[i] == 0)
                count++;
        }
        return count;
    }

    int CountSolvedEdges(CubeState state)
    {
        int count = 0;
        for (int i = 0; i < 12; i++)
        {
            if (state.EP[i] == i && state.EO[i] == 0)
                count++;
        }
        return count;
    }

    bool Prune(int depth, CubeState state)
    {
        if (depth == 1 && (CountSolvedCorners(state) < 4 || CountSolvedEdges(state) < 8))
            return true;
        if (depth == 2 && CountSolvedEdges(state) < 4)
            return true;
        if (depth == 3 && CountSolvedEdges(state) < 2)
            return true;
        return false;
    }

    public bool DepthLimitedSearch(CubeState state, int depth)
    {
        if (depth == 0 && IsSolved(state))
        {
            return true;
        }
        else if (depth == 0)
        {
            return false;
        }
        else if (Prune(depth, state))
        {
            return false;
        }

        string prevMove = _currentSolution.Count > 0 ? _currentSolution[_currentSolution.Count - 1] : null;
        foreach (var moveName in _moveNames)
        {
            if (!IsMoveAvailable(prevMove, moveName)) //全ての動かし方と前回の動かし方を比較して、同じ面を動かす場合は次のループにスキップ
                continue;

            _currentSolution.Add(moveName);
            if (DepthLimitedSearch(ApplyMove(state, Moves[moveName]), depth - 1))
                return true;
            _currentSolution.RemoveAt(_currentSolution.Count - 1);

            //たとえば3手で解ける場合、1手目でUを試し、2手目Rを試し、3手目でFを試したときに解けたら、その順序を記録しているので答えになる。
            //樹形図を作って上から全て試しているイメージ
        }
        return false;
    }

    public string StartSearch(CubeState state, int maxLength = 20)
    {
        for (int depth = 0; depth < maxLength; depth++)
        {
            if (DepthLimitedSearch(state, depth))
            {
                Debug.Log($"Solution found at depth {depth}");
                return string.Join(" ", _currentSolution);
            }
        }
        Debug.Log("No solution found");
        return null;
    }
}