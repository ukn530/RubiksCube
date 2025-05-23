using System;
using System.Collections.Generic;
using UnityEngine;

public class CubeModel
{
    Dictionary<string, CubeState> _moves = new Dictionary<string, CubeState>();
    public Dictionary<string, CubeState> Moves => _moves;
    List<string> _moveNames = new List<string>();
    Dictionary<string, int> _moveNamesToIndex = new Dictionary<string, int>();
    readonly List<string> _moveNamesPh2 = new List<string> { "U", "U2", "U'", "D", "D2", "D'", "L2", "R2", "F2", "B2" };
    Dictionary<string, int> _moveNamesToIndexPh2 = new Dictionary<string, int>();
    string[] _faces = new string[] { "U", "D", "L", "R", "F", "B" };
    List<string> _currentSolutionPh1 = new List<string>();

    List<string> _currentSolutionPh2 = new List<string>();
    int _maxSolutionLength = 23;
    string _bestSolution = null;
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
    const int NumCo = 2187; //3^7
    const int NumEo = 2048; //2^11
    const int NumECombinations = 495; //12C4
    const int NumCp = 40320; //8!
    const int NumUdEp = 40320; //8!
    const int NumEEp = 24; //4!

    int[,] _coMoveTable;
    int[,] _eoMoveTable;
    int[,] _eCombinationTable;
    int[,] _cpMoveTable;
    int[,] _udEpMoveTable;
    int[,] _eEpMoveTable;

    int[,] _coEecPruneTable;
    int[,] _eoEecPruneTable;
    int[,] _cpEEpPruneTable;
    int[,] _udEpEEpPruneTable;

    public CubeModel()
    {
        InitializeMoves();
        _coMoveTable = BuildCoMoveTable();
        _eoMoveTable = BuildEoMoveTable();
        _eCombinationTable = BuildECombinationMoveTable();
        _cpMoveTable = BuildCpMoveTable();
        _udEpMoveTable = BuildUdEpMoveTable();
        _eEpMoveTable = BuildEEpMoveTable();
        _coEecPruneTable = BuildCoEecPruneTable(_coMoveTable, _eCombinationTable);
        _eoEecPruneTable = BuildEoEecPruneTable(_eoMoveTable, _eCombinationTable);
        _cpEEpPruneTable = BuildCpEEpPruneTable(_cpMoveTable, _eEpMoveTable);
        _udEpEEpPruneTable = BuildUdEpEEpPruneTable(_udEpMoveTable, _eEpMoveTable);
    }

    public CubeState ApplyMove(CubeState state, CubeState move)
    {
        int[] newCP = new int[NumCorners];
        int[] newCO = new int[NumCorners];
        int[] newEP = new int[NumEdges];
        int[] newEO = new int[NumEdges];

        for (int i = 0; i < NumCorners; i++)
        {
            newCP[i] = state.CP[move.CP[i]];
            newCO[i] = (state.CO[move.CP[i]] + move.CO[i]) % 3;
        }
        for (int i = 0; i < NumEdges; i++)
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

        for (int i = 0; i < _moveNames.Count; i++)
        {
            _moveNamesToIndex[_moveNames[i]] = i;
        }

        for (int i = 0; i < _moveNamesPh2.Count; i++)
        {
            _moveNamesToIndexPh2[_moveNamesPh2[i]] = i;
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
        int[] co = new int[NumCorners];
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
        int[] eo = new int[NumEdges];
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
        int[] combination = new int[NumEdges];
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
        for (int i = 0; i < NumCorners; i++)
        {
            index *= NumCorners - i;
            for (int j = i + 1; j < NumCorners; j++)
            {
                if (cp[i] > cp[j])
                    index++;
            }
        }
        return index;
    }

    public int[] IndexToCp(int index)
    {
        int[] cp = new int[NumCorners];
        for (int i = 6; i >= 0; i--)
        {
            cp[i] = index % (NumCorners - i);
            index /= NumCorners - i;
            for (int j = i + 1; j < NumCorners; j++)
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
        for (int i = 0; i < NumCorners; i++)
        {
            index *= (NumCorners - i);
            for (int j = i + 1; j < NumCorners; j++)
            {
                if (ep[i] > ep[j])
                    index++;
            }
        }
        return index;
    }

    public int[] IndexToUdEp(int index)
    {
        int[] ep = new int[NumCorners];
        for (int i = 6; i >= 0; i--)
        {
            ep[i] = index % (NumCorners - i);
            index /= NumCorners - i;
            for (int j = i + 1; j < NumCorners; j++)
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
            index *= 4 - i;
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
            index /= 4 - i;
            for (int j = i + 1; j < 4; j++)
            {
                if (eep[j] >= eep[i])
                    eep[j]++;
            }
        }
        return eep;
    }

    //指定のCOのIndexに対して動かした時のCOのIndexを返す
    //例えば[10,1]だと、Indexが10のCOをIndexが1のmoveNameで動かした時のCOのIndexが入る
    //COの遷移表
    public int[,] BuildCoMoveTable()
    {
        int[,] coMoveTable = new int[NumCo, _moveNames.Count];
        for (int i = 0; i < NumCo; i++)
        {
            var state = new CubeState(
                new int[NumCorners],           // CP: all zeros
                IndexToCo(i),         // CO: from index
                new int[NumEdges],          // EP: all zeros
                new int[NumEdges]           // EO: all zeros
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

    //EOの遷移表
    public int[,] BuildEoMoveTable()
    {
        int[,] eoMoveTable = new int[NumEo, _moveNames.Count];
        for (int i = 0; i < NumEo; i++)
        {
            var state = new CubeState(
                new int[NumCorners],           // CP: all zeros
                new int[NumCorners],           // CO: all zeros
                new int[NumEdges],          // EP: all zeros
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

    //E列エッジの組合せの遷移表
    public int[,] BuildECombinationMoveTable()
    {
        int[,] eCombinationTable = new int[NumECombinations, _moveNames.Count];
        for (int i = 0; i < NumECombinations; i++)
        {
            var state = new CubeState(
                new int[NumCorners],                // CP: all zeros
                new int[NumCorners],                // CO: all zeros
                IndexToECombination(i),    // EP: from index
                new int[NumEdges]                // EO: all zeros
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

    //CPの遷移表
    public int[,] BuildCpMoveTable()
    {
        int[,] cpMoveTable = new int[NumCp, _moveNamesPh2.Count];
        for (int i = 0; i < NumCp; i++)
        {
            var state = new CubeState(
                IndexToCp(i),
                new int[NumCorners],
                new int[NumEdges],
                new int[NumEdges]
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

    //UD面エッジのEPの遷移表
    public int[,] BuildUdEpMoveTable()
    {
        int[,] udEpMoveTable = new int[NumUdEp, _moveNamesPh2.Count];
        for (int i = 0; i < NumUdEp; i++)
        {
            var ep = new int[NumEdges];
            var udEp = IndexToUdEp(i);
            for (int j = 0; j < NumCorners; j++)
                ep[j + 4] = udEp[j];

            var state = new CubeState(
                new int[NumCorners],    // CP: all zeros
                new int[NumCorners],    // CO: all zeros
                ep,            // EP: [0,0,0,0] + udEp[0..7]
                new int[NumEdges]    // EO: all zeros
            );

            for (int iMove = 0; iMove < _moveNamesPh2.Count; iMove++)
            {
                var moveName = _moveNamesPh2[iMove];
                var newState = ApplyMove(state, _moves[moveName]);
                int[] newUdEp = new int[NumCorners];
                for (int j = 0; j < NumCorners; j++)
                    newUdEp[j] = newState.EP[j + 4];
                udEpMoveTable[i, iMove] = UdEpToIndex(newUdEp);
            }
        }
        return udEpMoveTable;
    }

    //E列エッジのEPの遷移表
    public int[,] BuildEEpMoveTable()
    {
        int[,] eEpMoveTable = new int[NumEEp, _moveNamesPh2.Count];
        for (int i = 0; i < NumEEp; i++)
        {
            var eep = IndexToEEp(i);
            var ep = new int[NumEdges];
            for (int j = 0; j < 4; j++)
                ep[j] = eep[j];
            // ep[4..11] remain 0

            var state = new CubeState(
                new int[NumCorners],    // CP: all zeros
                new int[NumCorners],    // CO: all zeros
                ep,            // EP: eep[0..3] + 0s
                new int[NumEdges]    // EO: all zeros
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

    //EOを無視して、COとE列だけ考えたときの最短手数表
    public int[,] BuildCoEecPruneTable(int[,] coMoveTable, int[,] eCombinationTable)
    {
        int[,] coEecPruneTable = new int[NumCo, NumECombinations];
        for (int i = 0; i < NumCo; i++)
            for (int j = 0; j < NumECombinations; j++)
                coEecPruneTable[i, j] = -1;

        coEecPruneTable[0, 0] = 0;
        int distance = 0;
        int numFilled = 1;

        while (numFilled != NumCo * NumECombinations)
        {
            // Debug.Log($"distance = {distance}");
            // Debug.Log($"numFilled = {numFilled}");
            for (int iCo = 0; iCo < NumCo; iCo++)
            {
                for (int iEec = 0; iEec < NumECombinations; iEec++)
                {
                    if (coEecPruneTable[iCo, iEec] == distance)
                    {
                        for (int iMove = 0; iMove < _moveNames.Count; iMove++)
                        {
                            int nextCo = coMoveTable[iCo, iMove];
                            int nextEec = eCombinationTable[iEec, iMove];
                            if (coEecPruneTable[nextCo, nextEec] == -1)
                            {
                                coEecPruneTable[nextCo, nextEec] = distance + 1;
                                numFilled++;
                            }
                        }
                    }
                }
            }
            distance++;
        }
        return coEecPruneTable;
    }

    //COを無視して、EOとE列だけ考えたときの最短手数表
    public int[,] BuildEoEecPruneTable(int[,] eoMoveTable, int[,] eCombinationTable)
    {
        int[,] eoEecPruneTable = new int[NumEo, NumECombinations];
        for (int i = 0; i < NumEo; i++)
            for (int j = 0; j < NumECombinations; j++)
                eoEecPruneTable[i, j] = -1;

        eoEecPruneTable[0, 0] = 0;
        int distance = 0;
        int numFilled = 1;

        while (numFilled != NumEo * NumECombinations)
        {
            // Debug.Log($"distance = {distance}");
            // Debug.Log($"numFilled = {numFilled}");
            for (int iEo = 0; iEo < NumEo; iEo++)
            {
                for (int iEec = 0; iEec < NumECombinations; iEec++)
                {
                    if (eoEecPruneTable[iEo, iEec] == distance)
                    {
                        for (int iMove = 0; iMove < _moveNames.Count; iMove++)
                        {
                            int nextEo = eoMoveTable[iEo, iMove];
                            int nextEec = eCombinationTable[iEec, iMove];
                            if (eoEecPruneTable[nextEo, nextEec] == -1)
                            {
                                eoEecPruneTable[nextEo, nextEec] = distance + 1;
                                numFilled++;
                            }
                        }
                    }
                }
            }
            distance++;
        }
        return eoEecPruneTable;
    }

    //Phase2
    //UD面のエッジを無視して、CPとE列エッジだけ揃えるときの最短手数表
    public int[,] BuildCpEEpPruneTable(int[,] cpMoveTable, int[,] eEpMoveTable)
    {
        int[,] cpEEpPruneTable = new int[NumCp, NumEEp];
        for (int i = 0; i < NumCp; i++)
            for (int j = 0; j < NumEEp; j++)
                cpEEpPruneTable[i, j] = -1;

        cpEEpPruneTable[0, 0] = 0;
        int distance = 0;
        int numFilled = 1;

        while (numFilled != NumCp * NumEEp)
        {
            // Debug.Log($"distance = {distance}");
            // Debug.Log($"numFilled = {numFilled}");
            for (int iCp = 0; iCp < NumCp; iCp++)
            {
                for (int iEEp = 0; iEEp < NumEEp; iEEp++)
                {
                    if (cpEEpPruneTable[iCp, iEEp] == distance)
                    {
                        for (int iMove = 0; iMove < _moveNamesPh2.Count; iMove++)
                        {
                            int nextCp = cpMoveTable[iCp, iMove];
                            int nextEEp = eEpMoveTable[iEEp, iMove];
                            if (cpEEpPruneTable[nextCp, nextEEp] == -1)
                            {
                                cpEEpPruneTable[nextCp, nextEEp] = distance + 1;
                                numFilled++;
                            }
                        }
                    }
                }
            }
            distance++;
        }
        return cpEEpPruneTable;
    }

    //CPを無視して、UD面のエッジとE列エッジだけ揃えるときの最短手数表
    public int[,] BuildUdEpEEpPruneTable(int[,] udEpMoveTable, int[,] eEpMoveTable)
    {
        int[,] udepEepPruneTable = new int[NumUdEp, NumEEp];
        for (int i = 0; i < NumUdEp; i++)
            for (int j = 0; j < NumEEp; j++)
                udepEepPruneTable[i, j] = -1;

        udepEepPruneTable[0, 0] = 0;
        int distance = 0;
        int numFilled = 1;

        while (numFilled != NumUdEp * NumEEp)
        {
            // Debug.Log($"distance = {distance}");
            // Debug.Log($"numFilled = {numFilled}");
            for (int iUdEp = 0; iUdEp < NumUdEp; iUdEp++)
            {
                for (int iEEp = 0; iEEp < NumEEp; iEEp++)
                {
                    if (udepEepPruneTable[iUdEp, iEEp] == distance)
                    {
                        for (int iMove = 0; iMove < _moveNamesPh2.Count; iMove++)
                        {
                            int nextUdEp = udEpMoveTable[iUdEp, iMove];
                            int nextEEp = eEpMoveTable[iEEp, iMove];
                            if (udepEepPruneTable[nextUdEp, nextEEp] == -1)
                            {
                                udepEepPruneTable[nextUdEp, nextEEp] = distance + 1;
                                numFilled++;
                            }
                        }
                    }
                }
            }
            distance++;
        }
        return udepEepPruneTable;
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

    // --- Kociemba 2-phase Search Implementation ---
    public string StartSearch(CubeState initialState, int maxLength = 23)
    {
        _bestSolution = null;
        _maxSolutionLength = maxLength;
        _currentSolutionPh1.Clear();
        _currentSolutionPh2.Clear();
        StartSearchIterative(initialState, maxLength);
        return _bestSolution;
    }

    void StartSearchIterative(CubeState initialState, int maxLength)
    {
        _maxSolutionLength = maxLength;
        _bestSolution = null;
        int coIndex = CoToIndex(initialState.CO);
        int eoIndex = EoToIndex(initialState.EO);
        int[] eCombination = new int[12];
        for (int i = 0; i < 12; i++)
            eCombination[i] = (initialState.EP[i] >= 0 && initialState.EP[i] <= 3) ? 1 : 0;
        int eCombIndex = ECombinationToIndex(eCombination);

        for (int depth = 0; depth <= _maxSolutionLength; depth++)
        {
            _currentSolutionPh1.Clear();
            _currentSolutionPh2.Clear();
            DepthLimitedSearchPh1(initialState, coIndex, eoIndex, eCombIndex, depth);
            // if (DepthLimitedSearchPh1(initialState, coIndex, eoIndex, eCombIndex, depth))
            // {
            //     return;
            // }
        }
    }

    bool DepthLimitedSearchPh1(CubeState initialState, int coIndex, int eoIndex, int eCombIndex, int depth)
    {
        if (depth == 0 && coIndex == 0 && eoIndex == 0 && eCombIndex == 0)
        {
            string lastMove = _currentSolutionPh1.Count > 0 ? _currentSolutionPh1[_currentSolutionPh1.Count - 1] : null;
            if (lastMove == null || lastMove.StartsWith("R") || lastMove.StartsWith("L") || lastMove.StartsWith("F") || lastMove.StartsWith("B"))
            {
                CubeState state = initialState;
                foreach (var moveName in _currentSolutionPh1)
                    state = ApplyMove(state, Moves[moveName]);
                return SearchPhase2(state);
            }
        }
        if (depth == 0)
            return false;

        // Pruning
        if (Math.Max(_coEecPruneTable[coIndex, eCombIndex], _eoEecPruneTable[eoIndex, eCombIndex]) > depth)
            return false;

        string prevMove = _currentSolutionPh1.Count > 0 ? _currentSolutionPh1[_currentSolutionPh1.Count - 1] : null;
        foreach (var moveName in _moveNames)
        {
            if (!IsMoveAvailable(prevMove, moveName))
                continue;
            _currentSolutionPh1.Add(moveName);
            int moveIndex = _moveNamesToIndex[moveName];
            int nextCoIndex = _coMoveTable[coIndex, moveIndex];
            int nextEoIndex = _eoMoveTable[eoIndex, moveIndex];
            int nextECombIndex = _eCombinationTable[eCombIndex, moveIndex];
            if (DepthLimitedSearchPh1(initialState, nextCoIndex, nextEoIndex, nextECombIndex, depth - 1))
                return true;
            _currentSolutionPh1.RemoveAt(_currentSolutionPh1.Count - 1);
        }
        return false;
    }
    bool SearchPhase2(CubeState state)
    {
        int cpIndex = CpToIndex(state.CP);
        int[] udEp = new int[8];
        Array.Copy(state.EP, 4, udEp, 0, 8);
        int udepIndex = UdEpToIndex(udEp);
        int[] eep = new int[4];
        Array.Copy(state.EP, 0, eep, 0, 4);
        int eepIndex = EEpToIndex(eep);

        for (int depth = 0; depth <= _maxSolutionLength - _currentSolutionPh1.Count; depth++)
        {
            _currentSolutionPh2.Clear();
            if (DepthLimitedSearchPh2(cpIndex, udepIndex, eepIndex, depth))
                return true;
        }
        return false;
    }

    private bool DepthLimitedSearchPh2(int cpIndex, int udepIndex, int eepIndex, int depth)
    {
        if (depth == 0 && cpIndex == 0 && udepIndex == 0 && eepIndex == 0)
        {
            int totalLength = _currentSolutionPh1.Count + _currentSolutionPh2.Count;

            string solution = string.Join(" ", _currentSolutionPh1) + " . " + string.Join(" ", _currentSolutionPh2);
            Debug.Log($"Solution: {solution} ({totalLength} moves)");
            _maxSolutionLength = totalLength - 1;
            _bestSolution = solution;
            return true;
        }
        if (depth == 0)
            return false;

        // Pruning
        if (Math.Max(_cpEEpPruneTable[cpIndex, eepIndex], _udEpEEpPruneTable[udepIndex, eepIndex]) > depth)
            return false;

        string prevMove = null;
        if (_currentSolutionPh2.Count > 0)
            prevMove = _currentSolutionPh2[_currentSolutionPh2.Count - 1];
        else if (_currentSolutionPh1.Count > 0)
            prevMove = _currentSolutionPh1[_currentSolutionPh1.Count - 1];

        foreach (var moveName in _moveNamesPh2)
        {
            if (!IsMoveAvailable(prevMove, moveName))
                continue;
            _currentSolutionPh2.Add(moveName);
            int moveIndex = _moveNamesToIndexPh2[moveName];
            int nextCpIndex = _cpMoveTable[cpIndex, moveIndex];
            int nextUdEpIndex = _udEpMoveTable[udepIndex, moveIndex];
            int nextEEpIndex = _eEpMoveTable[eepIndex, moveIndex];
            if (DepthLimitedSearchPh2(nextCpIndex, nextUdEpIndex, nextEEpIndex, depth - 1))
                return true;
            _currentSolutionPh2.RemoveAt(_currentSolutionPh2.Count - 1);
        }
        return false;
    }
}