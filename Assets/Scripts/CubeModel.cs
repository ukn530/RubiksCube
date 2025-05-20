using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class CubeModel
{
    Dictionary<string, CubeState> _moves = new Dictionary<string, CubeState>();
    public Dictionary<string, CubeState> Moves => _moves;
    List<string> _moveNames = new List<string>();
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