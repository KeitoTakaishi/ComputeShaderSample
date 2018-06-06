using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class Instacing : MonoBehaviour {

	#region Defines
	//CompuuteShader用構造体
	private 
	const int ThreadBlockSize = 256;
	
	struct CubeData
	{
		public Vector3 Position;
		public Vector3 Velocity;
		public Vector3 Rotation;
		public Vector3 Albedo;
	}
	#endregion

	#region Serialize Field

	[SerializeField] private int _instanceCountX = 100;
	[SerializeField] private int _instanceCountY = 100;
	[SerializeField] private ComputeShader _ComputeShader;
	[SerializeField] private Mesh _CubeMesh;
	[SerializeField] private Material _CubeMaterial;
	[SerializeField] private Vector3 _CubeMeshScale = new Vector3(1, 1, 1);
	[SerializeField] private Vector3 boxScale = new Vector3(1.0f, 1.0f, 1.0f);
	//compute shader に渡すrender texture
	//[SerializeField] private RenderTexture _NoiseTexture;
	
	[SerializeField] Vector3 _BoundCenter = Vector3.zero;
	[SerializeField] Vector3 _BoundSize = new Vector3(300f, 300f, 300f);
	
	//振幅
	[SerializeField] private float amplitude = 1.0f;
	
	//gravity
	[SerializeField] 
	[Range(0, 10)]
	private float gravity = 9.8f;
	#endregion
	
	#region Private Field
	private ComputeBuffer _CubeDataBuffer;
	private ComputeBuffer _BaseCubeDataBuffer;
	private ComputeBuffer _PrevCubeDataBuffer;
	private ComputeBuffer _WaveBuffer;
	private ComputeBuffer _PrevWaveBuffer;
	
	
	// buffer for instacing
	private uint[] _GPUInstancingArgs = new uint[5];
	
	private ComputeBuffer _GPUInstancingArgsBuffer;
	//instance数
	private int _instanceCount;
	
	
	#endregion
	
	void Start ()
	{
		_instanceCount = _instanceCountX * _instanceCountY;

		// allocate buffers
		_CubeDataBuffer = new ComputeBuffer(_instanceCount, Marshal.SizeOf(typeof(CubeData)));
		_BaseCubeDataBuffer = new ComputeBuffer(_instanceCount, Marshal.SizeOf(typeof(CubeData)));
		_PrevCubeDataBuffer = new ComputeBuffer(_instanceCount, Marshal.SizeOf(typeof(CubeData)));
		//_WaveBuffer = new ComputeBuffer(_instanceCountX, Marshal.SizeOf(typeof(float)));
		//_PrevWaveBuffer = new ComputeBuffer(_instanceCountX, Marshal.SizeOf(typeof(float)));
		_GPUInstancingArgsBuffer = new ComputeBuffer(1, _GPUInstancingArgs.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
		//var cubeDataArr = new CubeData[_instanceCount];

		// init cube position
		int kernelId = _ComputeShader.FindKernel("Init");
		_ComputeShader.SetInt("_Width", _instanceCountX);
		_ComputeShader.SetInt("_Height", _instanceCountY);
		_ComputeShader.SetBuffer(kernelId, "_CubeDataBuffer", _CubeDataBuffer);
		_ComputeShader.SetBuffer(kernelId, "_BaseCubeDataBuffer", _BaseCubeDataBuffer);
		_ComputeShader.SetBuffer(kernelId, "_PrevCubeDataBuffer", _PrevCubeDataBuffer);
		//_ComputeShader.SetBuffer(kernelId, "_WaveBuffer", _WaveBuffer);
		//_ComputeShader.SetBuffer(kernelId, "_PrevWaveBuffer", _PrevWaveBuffer);
		_ComputeShader.Dispatch(kernelId, (Mathf.CeilToInt(_instanceCount / ThreadBlockSize) + 1), 1, 1);

//		kernelId = _ComputeShader.FindKernel("InitWave");
//		_ComputeShader.SetBuffer(kernelId, "_WaveBuffer", _WaveBuffer);
//		_ComputeShader.SetBuffer(kernelId, "_PrevWaveBuffer", _PrevWaveBuffer);
//		_ComputeShader.Dispatch(kernelId, (Mathf.CeilToInt(_instanceCountX / ThreadBlockSize) + 1), 1, 1);

		

	}
	
	
	void Update ()
	{
		
		
//		int kernelId;
//		kernelId = _ComputeShader.FindKernel("UpdateWave");
//		_ComputeShader.SetBuffer(kernelId, "_WaveBuffer", _WaveBuffer);
//		_ComputeShader.SetBuffer(kernelId, "_PreWaveBuffer", _PrevWaveBuffer);
//		_ComputeShader.Dispatch(kernelId, (Mathf.CeilToInt(_instanceCount / ThreadBlockSize) + 1), 1, 1);

		
		
		
		//GPU Instancing 
		_GPUInstancingArgs[0] = (_CubeMesh != null) ? _CubeMesh.GetIndexCount(0) : 0;
		_GPUInstancingArgs[1] = (uint)_instanceCount;
		_GPUInstancingArgsBuffer.SetData(_GPUInstancingArgs);
		
	
		_CubeMaterial.SetBuffer("_CubeDataBuffer", _CubeDataBuffer);
		_CubeMaterial.SetVector("_CubeMeshScale", _CubeMeshScale);
		_CubeMaterial.SetFloat("_time", Time.time);
		Graphics.DrawMeshInstancedIndirect(_CubeMesh, 0, _CubeMaterial, new Bounds(_BoundCenter, _BoundSize), 
			_GPUInstancingArgsBuffer);

	}

	private void OnDestroy()
	{
		if (this._CubeDataBuffer != null)
		{
			this._CubeDataBuffer.Release();
		}
		
		if (this._BaseCubeDataBuffer != null)
		{
			this._BaseCubeDataBuffer.Release();
		}
		
		if (this._PrevCubeDataBuffer != null)
		{
			this._PrevCubeDataBuffer.Release();
		}
		if (this._WaveBuffer != null)
		{
			this._WaveBuffer.Release();
			this._WaveBuffer = null;
		}
		if (this._PrevWaveBuffer != null)
		{
			this._PrevWaveBuffer.Release();
			this._PrevWaveBuffer = null;
		}
		if (this._GPUInstancingArgsBuffer != null)
		{
			this._GPUInstancingArgsBuffer.Release();
			this._GPUInstancingArgsBuffer = null;
		}

		
	}
}
