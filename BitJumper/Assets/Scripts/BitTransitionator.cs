using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class BitTransitionator : MonoBehaviour
{
    // Start is called before the first frame update
    private int _quality = 0; //0 = 8-bit, 1 = 16-bit, 2 = 64-bit, 3 = HD
    public PostProcessVolume _8Bit;
    public PostProcessVolume _16Bit;
    public PostProcessVolume _64Bit;
    public float Speed = 1;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float _time = Time.deltaTime * Speed;
        switch (_quality)
        {
            case 0:
                if (_8Bit.weight >= 1)
                {
                    _8Bit.weight = 1;
                    if (_16Bit.weight <= _time)
                    {
                        _16Bit.weight = 0;
                    }
                    else
                    {
                        _16Bit.weight -= _time;
                    }
                    if (_64Bit.weight <= _time)
                    {
                        _64Bit.weight = 0;
                    }
                    else
                    {
                        _64Bit.weight -= _time;
                    }
                }
                else
                {
                    _8Bit.weight += _time;
                }
                break;
            case 1:
                
                if (_16Bit.weight >= 1)
                {
                    _16Bit.weight = 1;
                    if (_8Bit.weight <= _time)
                    {
                        _8Bit.weight = 0;
                    }
                    else
                    {
                        _8Bit.weight -= _time;
                    }
                    if (_64Bit.weight <= _time)
                    {
                        _64Bit.weight = 0;
                    }
                    else
                    {
                        _64Bit.weight -= _time;
                    }
                }
                else
                {
                    _16Bit.weight += _time;
                }
                break;
            case 2:
                if (_64Bit.weight >= 1)
                {
                    _64Bit.weight = 1;
                    if (_8Bit.weight <= _time)
                    {
                        _8Bit.weight = 0;
                    }
                    else
                    {
                        _8Bit.weight -= _time;
                    }
                    if (_16Bit.weight <= _time)
                    {
                        _16Bit.weight = 0;
                    }
                    else
                    {
                        _16Bit.weight -= _time;
                    }
                }
                else
                {
                    _64Bit.weight += _time;
                }
                break;
            case 3:
                if (_8Bit.weight <= _time)
                {
                    _8Bit.weight = 0;
                }
                else
                {
                    _8Bit.weight -= _time;
                }
                if (_16Bit.weight <= _time)
                {
                    _16Bit.weight = 0;
                }
                else
                {
                    _16Bit.weight -= _time;
                }
                if (_64Bit.weight <= _time)
                {
                    _64Bit.weight = 0;
                }
                else
                {
                    _64Bit.weight -= _time;
                }
                break;
            default:
                if (_8Bit.weight >= 1)
                {
                    _8Bit.weight = 1;
                    if (_16Bit.weight <= _time)
                    {
                        _16Bit.weight = 0;
                    }
                    else
                    {
                        _16Bit.weight -= _time;
                    }
                    if (_64Bit.weight <= _time)
                    {
                        _64Bit.weight = 0;
                    }
                    else
                    {
                        _64Bit.weight -= _time;
                    }
                }
                else
                {
                    _8Bit.weight += _time;
                }
                break;
        }
        if (Input.GetKeyDown(KeyCode.G)) // Gameboy Post Processing (Pixelated, 8-bit Colors, High Contrast, Grayscale)
        {
            _quality = 0;
        }
        else if (Input.GetKeyDown(KeyCode.H)) // 16-Bit Post Processing (Pixelated, 16-bit Colors)
        {
            _quality = 1;
        }
        else if (Input.GetKeyDown(KeyCode.J)) // 64-Bit Post Processing (Pixelated, 64-bit Colors)
        {
            _quality = 2;
        }
        else if (Input.GetKeyDown(KeyCode.K)) // HD (No Post Processing Active)
        {
            _quality = 3;
        }
    }
}
