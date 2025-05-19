using System.Security.Cryptography;
using UnityEngine;

public class CubeState : MonoBehaviour
{
    // 角の位置（0～7）、角の回転（0～2）
    int[] _cp = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 }; // corner permutation
    int[] _co = new int[] { 0, 0, 0, 0, 0, 0, 0, 0 }; // corner orientation

    // 辺の位置（0～11）、辺の回転（0,1）
    int[] _ep = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 }; // edge permutation
    int[] _eo = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }; // edge orientation

    public CubeState(int[] newcp, int[] newco, int[] newep, int[] neweo)
    {
        _cp = newcp;
        _co = newco;
        _ep = newep;
        _eo = neweo;
    }

    public CubeState ApplyMove(CubeState move)
    {
        int[] new_cp = new int[8];
        int[] new_co = new int[8];
        int[] new_ep = new int[12];
        int[] new_eo = new int[12];

        for (int i = 0; i < 8; i++)
        {
            new_cp[i] = _cp[move._cp[i]];
            new_co[i] = (_co[move._cp[i]] + move._co[i]) % 3;
        }
        for (int i = 0; i < 12; i++)
        {
            new_ep[i] = _ep[move._ep[i]];
            new_eo[i] = (_eo[move._ep[i]] + move._eo[i]) % 2;
        }

        return new CubeState(new_cp, new_co, new_ep, new_eo);
    }


    void Start()
    {
        // InitializeCube();
        var r_state = new CubeState(
            new int[] { 0, 2, 6, 3, 4, 1, 5, 7 },
            new int[] { 0, 1, 2, 0, 0, 2, 1, 0 },
            new int[] { 0, 5, 9, 3, 4, 2, 6, 7, 8, 1, 10, 11 },
            new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
        );

        var r2_state = r_state.ApplyMove(r_state);
        Debug.Log("r2_state.cp: " + string.Join(", ", r2_state._cp));
        Debug.Log("r2_state.co: " + string.Join(", ", r2_state._co));
        Debug.Log("r2_state.ep: " + string.Join(", ", r2_state._ep));
        Debug.Log("r2_state.eo: " + string.Join(", ", r2_state._eo));
    }

    // 初期状態（完成状態）にリセット
    public void InitializeCube()
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

    // 各面の操作
    public void Move(string move)
    {
        switch (move)
        {
            case "U": U(); break;
            case "U2": U(); U(); break;
            case "U'": U(); U(); U(); break;
            case "D": D(); break;
            case "D2": D(); D(); break;
            case "D'": D(); D(); D(); break;
            case "F": F(); break;
            case "F2": F(); F(); break;
            case "F'": F(); F(); F(); break;
            case "B": B(); break;
            case "B2": B(); B(); break;
            case "B'": B(); B(); B(); break;
            case "L": L(); break;
            case "L2": L(); L(); break;
            case "L'": L(); L(); L(); break;
            case "R": R(); break;
            case "R2": R(); R(); break;
            case "R'": R(); R(); R(); break;
        }
    }

    // 各面の90度回転（例：U面）
    void U()
    {
        // 角
        Rotate(_cp, 0, 1, 2, 3);
        Rotate(_co, 0, 1, 2, 3);
        // 辺
        Rotate(_ep, 0, 1, 2, 3);
        Rotate(_eo, 0, 1, 2, 3);
    }
    void D()
    {
        Rotate(_cp, 4, 7, 6, 5);
        Rotate(_co, 4, 7, 6, 5);
        Rotate(_ep, 8, 11, 10, 9);
        Rotate(_eo, 8, 11, 10, 9);
    }
    void F()
    {
        Rotate(_cp, 0, 3, 7, 4);
        RotateWithCO(0, 3, 7, 4, 1, 2, 1, 2); // F面は角の向きが変わる
        Rotate(_ep, 1, 7, 9, 4);
        RotateWithEO(1, 7, 9, 4);
    }
    void B()
    {
        Rotate(_cp, 2, 1, 5, 6);
        RotateWithCO(2, 1, 5, 6, 1, 2, 1, 2);
        Rotate(_ep, 3, 5, 11, 6);
        RotateWithEO(3, 5, 11, 6);
    }
    void L()
    {
        Rotate(_cp, 1, 0, 4, 5);
        RotateWithCO(1, 0, 4, 5, 2, 1, 2, 1);
        Rotate(_ep, 0, 4, 8, 5);
        RotateWithEO(0, 4, 8, 5);
    }
    void R()
    {
        Rotate(_cp, 3, 2, 6, 7);
        RotateWithCO(3, 2, 6, 7, 2, 1, 2, 1);
        Rotate(_ep, 2, 6, 10, 7);
        RotateWithEO(2, 6, 10, 7);
    }

    // 配列の4点サイクル
    void Rotate(int[] arr, int a, int b, int c, int d)
    {
        int tmp = arr[a];
        arr[a] = arr[d];
        arr[d] = arr[c];
        arr[c] = arr[b];
        arr[b] = tmp;
    }

    // 角の回転を考慮したサイクル
    void RotateWithCO(int a, int b, int c, int d, int oa, int ob, int oc, int od)
    {
        int[] tmp = { _co[a], _co[b], _co[c], _co[d] };
        _co[a] = (tmp[3] + oa) % 3;
        _co[b] = (tmp[0] + ob) % 3;
        _co[c] = (tmp[1] + oc) % 3;
        _co[d] = (tmp[2] + od) % 3;
    }

    // 辺の回転を考慮したサイクル
    void RotateWithEO(int a, int b, int c, int d)
    {
        int[] tmp = { _eo[a], _eo[b], _eo[c], _eo[d] };
        _eo[a] = (tmp[3] + 1) % 2;
        _eo[b] = (tmp[0] + 1) % 2;
        _eo[c] = (tmp[1] + 1) % 2;
        _eo[d] = (tmp[2] + 1) % 2;
    }
}