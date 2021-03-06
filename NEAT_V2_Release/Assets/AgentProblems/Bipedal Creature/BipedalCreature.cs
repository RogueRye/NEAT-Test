﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BipedalCreature : MonoBehaviour, IAgentTester
{
    /// <summary>
    /// Event subscriptions to notify controller when test is finished
    /// </summary>
    /// <param name="source">Source of the event (this)</param>
    /// <param name="args">Nothing</param>
    public delegate void TestFinishedEventHandler(object source, EventArgs args);
    public event TestFinishedEventHandler TestFinished;

    private bool isActive = false; // is this agent active
    private bool finished = false; // is this agent finished.  Making sure only 1 event is sent.

    private NEATNet net; //The brain

    private const string ACTION_ON_FINISHED = "OnFinished"; //On finished method

    private NEATGeneticControllerV2 controller; //Controller

    HingeJoint2D[] joints;
    GameObject[] kids;
    float startX;
    /// <summary>
    /// Set Color to this agent. Looks visually pleasing and may help in debugging? 
    /// </summary>
    /// <param name="color"> color</param>
    public void SetColor(Color color)
    {
        Renderer[] childRends = transform.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < childRends.Length; i++)
        {
            childRends[i].material.color = color;
        }
    }

    /// <summary>
    /// Start up tasks for this agent game object.
    /// </summary>
    void Start()
    {
        joints = GetComponentsInChildren<HingeJoint2D>();
      
        Physics2D.IgnoreLayerCollision(10, 10, true);

        startX = transform.position.x;
    }

    /// <summary>
    /// Tick
    /// </summary>
    public void UpdateNet()
    {
        float[] inputs = new float[12]; // 0-3 previous joint speed
                                        //4-7 max angle flag
                                        // 8-11 current joint angle    

        for(int i = 0; i <= 3; i++)
        {
            inputs[i] = joints[i].jointSpeed / joints[i].motor.maxMotorTorque;
        }

        for (int i = 4, j = 0; i <= 7; i++, j++)
        {
            inputs[i] = joints[j].limitState == JointLimitState2D.UpperLimit? 0: 1;
        }

        for(int i = 8, j = 0; i <= 11; i++, j++)
        {
            inputs[i] = joints[j].jointAngle;
        }
        var deltaX = transform.position.x - startX;

        float[] output = net.FireNet(inputs);

        for(int i = 0; i < 4; i++)
        {
            var motor = joints[i].motor;
            motor.motorSpeed = output[i] * 350;

            joints[i].motor = motor;
            joints[i].useMotor = true;

        }

        net.AddNetFitness(transform.GetChild(0).position.x);

    }

    /// <summary>
    /// Some fail condition for this agent
    /// </summary>
    /// <returns></returns>
    public bool FailCheck()
    {
        return false;
    }

    /// <summary>
    /// Fitness update per tick. Does not have to happen here! But good practice.
    /// </summary>
    public void CalculateFitnessOnUpdate()
    {

    }

    /// <summary>
    /// Final fitness calculation once this agent is finished or failed
    /// </summary>
    public void CalculateFitnessOnFinish()
    {

    }

    /// <summary>
    /// No need to worry about this method! You just need to code in UpdateNet and CalculateFitnessOnUpdate :D
    /// </summary>
    void FixedUpdate()
    {
        if (isActive == true)
        {
            UpdateNet(); //update neural net
            CalculateFitnessOnUpdate(); //calculate fitness

            if (FailCheck() == true)
            {
                OnFinished();
            }
        }
    }



    /// <summary>
    /// OnFinished is called when we want to notify controller this agent is done. 
    /// Automatically handels notification.
    /// </summary>
    public void OnFinished()
    {
        if (TestFinished != null)
        {
            if (!finished)
            {
                finished = true;
                CalculateFitnessOnFinish();
                TestFinished(net.GetNetID(), EventArgs.Empty);
                TestFinished -= controller.OnFinished; //unsubscrive from the event notification
                Destroy(gameObject); //destroy this gameobject
            }
        }
    }

    /// <summary>
    /// Activated the agent when controller give it a brain. 
    /// </summary>
    /// <param name="net">The brain</param>
    public void Activate(NEATNet net)
    {
        this.net = net;
        Invoke(ACTION_ON_FINISHED, net.GetTestTime());
        isActive = true;
    }

    /// <summary>
    /// Getting net. 
    /// This could be used by some other objects that have reference to this game object 
    /// and want to see the brain.
    /// </summary>
    /// <returns> The brain</returns>
    public NEATNet GetNet()
    {
        return net;
    }

    /// <summary>
    /// Adds controller and subscribes to an event listener in controller
    /// </summary>
    /// <param name="controller">Controller</param>
    public void SubscriveToEvent(NEATGeneticControllerV2 controller)
    {
        this.controller = controller;
        TestFinished += controller.OnFinished; //subscrive to an event notification
    }


}
