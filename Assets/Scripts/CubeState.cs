using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class CubeState
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

    Dictionary<string, CubeState> _moves = new Dictionary<string, CubeState>();
    public Dictionary<string, CubeState> Moves => _moves;
    List<string> _moveNames = new List<string>();
    string[] _faces = new string[] { "U", "D", "L", "R", "F", "B" };

    public CubeState()
    {
        InitializeCube();
        InitializeMoves();
    }

    public CubeState(int[] newcp, int[] newco, int[] newep, int[] neweo)
    {
        _cp = newcp;
        _co = newco;
        _ep = newep;
        _eo = neweo;
    }

    public CubeState ApplyMove(CubeState move)
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

        return new CubeState(newCP, newCO, newEP, newEO);
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

            _moves[face + r180] = _moves[face].ApplyMove(_moves[face]);
            _moves[face + r270] = _moves[face].ApplyMove(_moves[face]).ApplyMove(_moves[face]);
        }
    }

    public CubeState ScrambleToState(string scramble)
    {
        CubeState scrambledState = new CubeState(_cp, _co, _ep, _eo);
        foreach (var moveName in scramble.Split(' '))
        {
            var moveState = _moves[moveName];
            scrambledState = scrambledState.ApplyMove(moveState);
        }
        return scrambledState;
    }
}