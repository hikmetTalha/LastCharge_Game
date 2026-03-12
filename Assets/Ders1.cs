using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ders1 : MonoBehaviour
{
    public int para = 100;
    public int canHakki = 100;
    public float can = 100f;
    public bool paraDegistiMi = false;
    public string nicname;
    public GameObject mevcutObje;
    public Vector3 newPosition;
    void Start()
    {
        mevcutObje.GetComponent<CharacterMovement>().speed = 10;
        canHakki = 3;
        newPosition = new Vector3(0f, 0f, 2f);
    }

  
    void Update()
    {
        transform.Translate(newPosition);
    }
    public void ParaDegeriniDegistir(int paraDegeri , bool paraDegisti)
    {
        para = paraDegeri;
        paraDegistiMi = paraDegisti;
    }
    void isimDegistir(GameObject obje, string isim)
    {
        obje.name = isim;
    }
}
