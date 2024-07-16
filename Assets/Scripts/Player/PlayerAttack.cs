using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Player _Player;
    public Transform weaponSpawnPoint;

    [Header("Suica")]
    public GameObject suicaWeaponPrefab;
    private SuicaWeapon _SuicaWeapon;
    public float suicaFireRate = 0.5f;
    private float nextSuicaFireTime = 0f;
    [Header("LaserBeam")]
    public GameObject laserWeaponPrefab;
    private LaserWeapon[] _LaserWeapon = new LaserWeapon[4];
    public float laserFireRate = 0.75f;
    private float nextLaserFireTime = 0f;
    private int numberOfLaserWeapons = 4;
    [Header("Chart")]
    public GameObject chartWeaponPrefab;
    private ChartWeapon _ChartWeapon;
    private int numberOfRotatingWeapons = 3;
    [Header("SetSquare")]
    public GameObject setSquareWeaponPrefab;
    private SetSquareWeapon _SetSquareWeapon;
    public float setSquareFireRate = 0.5f;
    private float nextSetSquareFireTime = 0f;
    [Header("Portion")]
    public GameObject portionWeaponPrefab;
    private PortionWeapon _PortionWeapon;
    public float portionFireRate = 5f;
    private float nextPortionFireTime = 0f;

    [Header("Audio")]
    public AudioSource playerAudioSource;
    public AudioClip chartRotateSoundClip;
    public AudioClip laserBeamSoundClip;
    public AudioClip fireSuicaSoundClip;

    public void GenerateInitialWeapon()
    {
        // 初期の回転武器を生成
        for (int i = 0; i < numberOfRotatingWeapons; i++)
        {
            float angle = i * (360f / numberOfRotatingWeapons);
            GenerateWeapon(WeaponType.Chart, angle);
        }

        GenerateWeapon(WeaponType.Suica);

        for (int i = 0; i < numberOfLaserWeapons; i++)
        {
            int lazerWeaponsCount = i;
            GenerateWeapon(WeaponType.Laser, 0, lazerWeaponsCount);
        }

        GenerateWeapon(WeaponType.SetSquare);
        GenerateWeapon(WeaponType.Portion);
    }

    private void GenerateWeapon(WeaponType weaponType, float startingAngle = 0f, int lazerWeaponsCount = 0)
    {
        GameObject weaponPrefab = null;

        switch (weaponType)
        {
            case WeaponType.Suica:
                weaponPrefab = suicaWeaponPrefab;
                break;
            case WeaponType.Laser:
                weaponPrefab = laserWeaponPrefab;
                break;
            case WeaponType.Chart:
                weaponPrefab = chartWeaponPrefab;
                break;
            case WeaponType.SetSquare:
                weaponPrefab = setSquareWeaponPrefab;
                break;
            case WeaponType.Portion:
                weaponPrefab = portionWeaponPrefab;
                break;
        }

        if (weaponPrefab != null)
        {
            GameObject weapon = Instantiate(weaponPrefab, weaponSpawnPoint.position, Quaternion.identity,weaponSpawnPoint);
            Weapon weaponScript = weapon.GetComponent<Weapon>();

            switch (weaponType)
            {
                case WeaponType.Suica:
                    _SuicaWeapon = weapon.GetComponent<SuicaWeapon>();
                    break;
                case WeaponType.Laser:
                    _LaserWeapon[lazerWeaponsCount] = weapon.GetComponent<LaserWeapon>();
                    //レーザーの場合、向きを設定
                    _LaserWeapon[lazerWeaponsCount].direction = _LaserWeapon[lazerWeaponsCount].directions[lazerWeaponsCount]; 
                    break;
                case WeaponType.Chart:
                    // 回転武器の場合、初期角度を設定
                    _ChartWeapon = weapon.GetComponent<ChartWeapon>();
                    _ChartWeapon.startingAngle = startingAngle;
                    StartCoroutine(PlayChartRotateSound());
                    break;
                case WeaponType.SetSquare:
                    _SetSquareWeapon = weapon.GetComponent<SetSquareWeapon>();
                    break;
                case WeaponType.Portion:
                    _PortionWeapon = weapon.GetComponent<PortionWeapon>();
                    break;
            }

            weaponScript.Initialize(_Player.playerTransform);
        }
    }

    public void AddChartWeapon()
    {
        numberOfRotatingWeapons++;
        for (int i = 0; i < numberOfRotatingWeapons; i++)
        {
            float angle = i * (360f / numberOfRotatingWeapons);
            GenerateWeapon(WeaponType.Chart, angle);
        }
    }

    void Update()
    {
        //開始まで待つ
        if (!GameManager.Instance.IsGameStarted()) return;

        HandleShooting();
    }

    void HandleShooting()
    {
        // Suicaを発射するタイミングをチェック
        if (Time.time > nextSuicaFireTime)
        {
            FireSuica();
            nextSuicaFireTime = Time.time + suicaFireRate;
        }

        // レーザービームを発射するタイミングをチェック
        if (Time.time > nextLaserFireTime)
        {
            FireLaser();
            nextLaserFireTime = Time.time + laserFireRate;
        }

        //三角定規を発射するタイミングをチェック
        if (Time.time > nextSetSquareFireTime)
        {
            FireSetSquare();
            nextSetSquareFireTime = Time.time + setSquareFireRate;
        }

        //ポーションを発射するタイミングをチェック
        if (Time.time > nextPortionFireTime)
        {
            FirePortion();
            nextPortionFireTime = Time.time + portionFireRate;
        }
    }

    void FireSuica()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition - transform.position).normalized;

        if(_SuicaWeapon != null)
        {
            // Suicaを発射
            _SuicaWeapon.Fire(direction, transform);

            playerAudioSource.PlayOneShot(fireSuicaSoundClip, 0.4f);
        }
    }

    void FireLaser()
    {
        if (_LaserWeapon != null)
        {
            // レーザービームを発射
            for(int i = 0; i < numberOfLaserWeapons; i++)
            {
                _LaserWeapon[i].Fire();
            }

            playerAudioSource.PlayOneShot(laserBeamSoundClip, 0.15f);
        }
    }

    void FireSetSquare()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mousePosition - transform.position).normalized;
        _SetSquareWeapon.Fire(direction, transform);
    }

    void FirePortion()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _PortionWeapon.Fire(mousePosition, transform);
    }

    private IEnumerator PlayChartRotateSound()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            playerAudioSource.PlayOneShot(chartRotateSoundClip,0.08f);
        }
    }
}
