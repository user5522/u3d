using System.Collections;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public GameObject weapon;
    public float attackCooldown;
    public float withdrawDelay;
    public Animator weaponAnimator;
    public float chainAttackWindow;

    private bool drewWeapon = false;
    private bool canAttack = false;
    private Coroutine withdrawCoroutine;
    private Coroutine chainAttackCoroutine;
    private Coroutine cooldownCoroutine;
    private bool canChainAttack = false;
    private int currentAttackIndex = 0;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!drewWeapon) DrawWeapon();
            else if (drewWeapon && (canAttack || canChainAttack)) PerformAttack();
        }
    }

    private void DrawWeapon()
    {
        weaponAnimator.SetTrigger("Draw");
        drewWeapon = true;
        canAttack = true;
        ResetWithdrawTimer();
    }

    private void PerformAttack()
    {
        if (canAttack)
        {
            currentAttackIndex = 1;
            canAttack = false;
            ExecuteAttack();
        }
        else if (canChainAttack)
        {
            currentAttackIndex++;
            ExecuteAttack();
        }
    }

    private void ExecuteAttack()
    {
        switch (currentAttackIndex)
        {
            case 1:
                weaponAnimator.SetTrigger("Attack1");
                break;
            case 2:
                weaponAnimator.SetTrigger("Attack2");
                break;
            case 3:
                weaponAnimator.SetTrigger("Attack3");
                break;
        }
        ResetWithdrawTimer();
        canChainAttack = currentAttackIndex < 3;
        if (chainAttackCoroutine != null) StopCoroutine(chainAttackCoroutine);
        chainAttackCoroutine = StartCoroutine(HandleAttackChain());
    }

    private void WithdrawWeapon()
    {
        weaponAnimator.SetTrigger("Withdraw");
        drewWeapon = false;
        canAttack = false;
        canChainAttack = false;
        currentAttackIndex = 0;
    }

    private void ResetWithdrawTimer()
    {
        if (withdrawCoroutine != null) StopCoroutine(withdrawCoroutine);
        withdrawCoroutine = StartCoroutine(WithdrawAfterDelay());
    }

    private void StartCooldown()
    {
        if (cooldownCoroutine != null) StopCoroutine(cooldownCoroutine);
        cooldownCoroutine = StartCoroutine(AttackCooldown());
    }

    IEnumerator WithdrawAfterDelay()
    {
        yield return new WaitForSeconds(withdrawDelay);
        if (drewWeapon) WithdrawWeapon();
    }

    IEnumerator HandleAttackChain()
    {
        canChainAttack = true;
        yield return new WaitForSeconds(chainAttackWindow);
        canChainAttack = false;
        if (currentAttackIndex == 3 || !canChainAttack) StartCooldown();
    }

    IEnumerator AttackCooldown()
    {
        canAttack = false;
        canChainAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
        currentAttackIndex = 0;
    }
}