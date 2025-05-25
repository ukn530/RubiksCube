using System;
using System.Collections.Generic;
using UnityEngine;

public class CubeSearch
{
    List<string> _currentSolutionPh1 = new List<string>();
    List<string> _currentSolutionPh2 = new List<string>();
    int _maxSolutionLength = 23;
    string _bestSolution = null;
    float _timer = 0;

    Dictionary<string, string> _invFace = new Dictionary<string, string>
    {
        { "U", "D" },
        { "D", "U" },
        { "L", "R" },
        { "R", "L" },
        { "F", "B" },
        { "B", "F" }
    };

    CubeModel _cubeModel;

    public CubeSearch(CubeModel cubeModel)
    {
        _cubeModel = cubeModel;
    }

    // --- Kociemba 2-phase Search Implementation ---
    async public Awaitable<string> StartSearch(CubeState initialState, int maxLength = 23, float timeout = 1f)
    {
        _timer = 0;
        _bestSolution = null;
        _maxSolutionLength = maxLength;
        _currentSolutionPh1.Clear();
        _currentSolutionPh2.Clear();
        _bestSolution = await StartSearchIterative(initialState, maxLength, timeout);
        return _bestSolution;
    }

    async Awaitable<string> StartSearchIterative(CubeState initialState, int maxLength, float timeout = 1f)
    {
        _maxSolutionLength = maxLength;
        _bestSolution = null;
        int coIndex = _cubeModel.CoToIndex(initialState.CO);
        int eoIndex = _cubeModel.EoToIndex(initialState.EO);
        int[] eCombination = new int[12];
        for (int i = 0; i < 12; i++)
            eCombination[i] = (initialState.EP[i] >= 0 && initialState.EP[i] <= 3) ? 1 : 0;
        int eCombIndex = _cubeModel.ECombinationToIndex(eCombination);

        for (int depth = 0; depth <= _maxSolutionLength; depth++)
        {
            Debug.Log($"Searching ph1 depth/_maxSolutionLength: {depth}" + "/" + _maxSolutionLength);
            await Awaitable.NextFrameAsync();
            _currentSolutionPh1.Clear();
            _currentSolutionPh2.Clear();
            await DepthLimitedSearchPh1(initialState, coIndex, eoIndex, eCombIndex, depth, timeout);
            if (_timer > timeout) // Timeout check
            {
                Debug.Log("Search timed out. ph1");
                return _bestSolution;
            }
        }
        return _bestSolution;
    }

    async Awaitable<bool> DepthLimitedSearchPh1(CubeState initialState, int coIndex, int eoIndex, int eCombIndex, int depth, float timeout = 1f)
    {
        if (depth == 0 && coIndex == 0 && eoIndex == 0 && eCombIndex == 0)
        {
            string lastMove = _currentSolutionPh1.Count > 0 ? _currentSolutionPh1[_currentSolutionPh1.Count - 1] : null;
            if (lastMove == null || lastMove.StartsWith("R") || lastMove.StartsWith("L") || lastMove.StartsWith("F") || lastMove.StartsWith("B"))
            {
                CubeState state = initialState;
                foreach (var moveName in _currentSolutionPh1)
                    state = _cubeModel.ApplyMove(state, _cubeModel.Moves[moveName]);
                return await SearchPhase2(state, timeout);
            }
        }
        if (depth == 0)
            return false;

        // Pruning
        if (Math.Max(_cubeModel.CoEecPruneTable[coIndex, eCombIndex], _cubeModel.EoEecPruneTable[eoIndex, eCombIndex]) > depth)
            return false;

        string prevMove = _currentSolutionPh1.Count > 0 ? _currentSolutionPh1[_currentSolutionPh1.Count - 1] : null;
        foreach (var moveName in _cubeModel.MoveNames)
        {
            if (!IsMoveAvailable(prevMove, moveName))
                continue;
            _currentSolutionPh1.Add(moveName);
            int moveIndex = _cubeModel.MoveNamesToIndex[moveName];
            int nextCoIndex = _cubeModel.CoMoveTable[coIndex, moveIndex];
            int nextEoIndex = _cubeModel.EoMoveTable[eoIndex, moveIndex];
            int nextECombIndex = _cubeModel.ECombinationTable[eCombIndex, moveIndex];
            if (await DepthLimitedSearchPh1(initialState, nextCoIndex, nextEoIndex, nextECombIndex, depth - 1, timeout))
                return true;
            _currentSolutionPh1.RemoveAt(_currentSolutionPh1.Count - 1);

            if (_timer > timeout) // Timeout check
            {
                Debug.Log("Search timed out. ph1.1" + _timer);
                return false;
            }
        }
        return false;
    }
    async Awaitable<bool> SearchPhase2(CubeState state, float timeout = 1f)
    {
        int cpIndex = _cubeModel.CpToIndex(state.CP);
        int[] udEp = new int[8];
        Array.Copy(state.EP, 4, udEp, 0, 8);
        int udepIndex = _cubeModel.UdEpToIndex(udEp);
        int[] eep = new int[4];
        Array.Copy(state.EP, 0, eep, 0, 4);
        int eepIndex = _cubeModel.EEpToIndex(eep);

        for (int depth = 0; depth <= _maxSolutionLength - _currentSolutionPh1.Count; depth++)
        {
            Debug.Log($"Searching ph2 depth/_maxSolutionLength - _currentSolutionPh1.Count: {depth}" + "/" + (_maxSolutionLength - _currentSolutionPh1.Count));
            await Awaitable.NextFrameAsync();
            _currentSolutionPh2.Clear();
            _timer += Time.deltaTime;
            if (_timer > timeout) // Timeout check
            {
                Debug.Log("Search timed out. ph2" + _timer);
                return false;
            }
            if (DepthLimitedSearchPh2(cpIndex, udepIndex, eepIndex, depth))
                return true;
        }
        return false;
    }

    private bool DepthLimitedSearchPh2(int cpIndex, int udepIndex, int eepIndex, int depth, float timeout = 1f)
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
        if (Math.Max(_cubeModel.CpEEpPruneTable[cpIndex, eepIndex], _cubeModel.UdEpEEpPruneTable[udepIndex, eepIndex]) > depth)
            return false;

        string prevMove = null;
        if (_currentSolutionPh2.Count > 0)
            prevMove = _currentSolutionPh2[_currentSolutionPh2.Count - 1];
        else if (_currentSolutionPh1.Count > 0)
            prevMove = _currentSolutionPh1[_currentSolutionPh1.Count - 1];

        foreach (var moveName in _cubeModel.MoveNamesPh2)
        {
            if (!IsMoveAvailable(prevMove, moveName))
                continue;
            _currentSolutionPh2.Add(moveName);
            int moveIndex = _cubeModel.MoveNamesToIndexPh2[moveName];
            int nextCpIndex = _cubeModel.CpMoveTable[cpIndex, moveIndex];
            int nextUdEpIndex = _cubeModel.UdEpMoveTable[udepIndex, moveIndex];
            int nextEEpIndex = _cubeModel.EEpMoveTable[eepIndex, moveIndex];
            if (DepthLimitedSearchPh2(nextCpIndex, nextUdEpIndex, nextEEpIndex, depth - 1))
                return true;
            _currentSolutionPh2.RemoveAt(_currentSolutionPh2.Count - 1);
        }
        return false;
    }



    bool IsMoveAvailable(string prevMove, string move)
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
}
