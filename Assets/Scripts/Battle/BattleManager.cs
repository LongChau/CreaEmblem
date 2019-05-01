﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public struct LevelInfo
{
    public int prevExp;
    public int newExp;
    public int prevLevel;
    public int newLevel;
    public Hero hero;
}

public class BattleEvent : UnityEvent<Hero, Hero> { }
public class LevelEvent : UnityEvent<LevelInfo> { }

public class BattleManager : MonoBehaviour
{

    public BattleEvent onStartBattle = new BattleEvent();
    public BattleEvent onHeroAttack = new BattleEvent();
    public UnityEvent onEndCutscene = new UnityEvent();
    public UnityEvent onEndBattle = new UnityEvent();
    public LevelEvent onExpGain = new LevelEvent();

    bool continueBattle = false;

    Coroutine battleCoroutine = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Attack(Hero _attacker, Hero _attacked)
    {
        //Debug.Log(_attacker.name + " attack " + _attacked.name);
        if(battleCoroutine == null)
        {
            battleCoroutine = StartCoroutine(BattleCoroutine(_attacker, _attacked));
        }
    }

    // Can be called from animation
    public void ContinueBattle()
    {
        continueBattle = true;
    }

    IEnumerator BattleCoroutine(Hero _attacker, Hero _attacked)
    {
        DataManager.instance.blockInput = true;
        //DataManager.instance.inBattle = true;
        DataManager.instance.gameState = GameState.Battle;
        onStartBattle.Invoke(_attacker, _attacked);
        
        continueBattle = false;
        yield return new WaitUntil(() => continueBattle);

        // First the attacker attack
        onHeroAttack.Invoke(_attacker, _attacked);
        
        continueBattle = false;
        yield return new WaitUntil(() => continueBattle);

        // If the attacked is alive, then attack
        if(_attacked.isAlive)
        {
            onHeroAttack.Invoke(_attacked, _attacker);

            continueBattle = false;
            yield return new WaitUntil(() => continueBattle);
        }

        // Check for speed superiority : 
        // if one of them has 5 more in speed than the other
        // then attack a second time in the turn
        if(_attacker.speed >= _attacked.speed + 5)
        {
            onHeroAttack.Invoke(_attacker, _attacked);

            continueBattle = false;
            yield return new WaitUntil(() => continueBattle);
        }
        else if(_attacked.speed >= _attacker.speed + 5)
        {
            onHeroAttack.Invoke(_attacked, _attacker);

            continueBattle = false;
            yield return new WaitUntil(() => continueBattle);
        }

        battleCoroutine = null;
        onEndCutscene.Invoke();

        // Wait the end of animation
        continueBattle = false;
        yield return new WaitUntil(() => continueBattle);
        
        if(_attacker.isAlive && _attacker.expChanged)
        {
            LevelInfo info = _attacker.CheckCurrentLevel();
            onExpGain.Invoke(info);

            //Debug.Log("Attacker gain exp ! from " + info.prevExp + " to " + info.newExp);
            //if(info.newLevel != info.prevLevel)
            //{
            //    Debug.Log("Attacker gain level ! from " + info.prevLevel + " to " + info.newLevel);
            //}

            continueBattle = false;
            yield return new WaitUntil(() => continueBattle);
        }

        if (_attacked.isAlive && _attacked.expChanged)
        {
            LevelInfo info = _attacked.CheckCurrentLevel();
            onExpGain.Invoke(info);

            //Debug.Log("Attacked gain exp ! from " + info.prevExp + " to " + info.newExp);
            //if (info.newLevel != info.prevLevel)
            //{
            //    Debug.Log("Attacked gain level ! from " + info.prevLevel + " to " + info.newLevel);
            //}

            continueBattle = false;
            yield return new WaitUntil(() => continueBattle);
        }

        DataManager.instance.blockInput = false;
        DataManager.instance.gameState = GameState.Playing;
        onEndBattle.Invoke();
    }
}
