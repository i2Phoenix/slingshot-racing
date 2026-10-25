using UnityEngine;

public static class ParticleUtils
{
	public static void Emit(ParticleEmitter _emitter, Transform _transform, float _radius)
	{
		int numParticles = Mathf.RoundToInt(Random.Range(_emitter.minEmission, _emitter.maxEmission));
		Emit(_emitter, _transform, _radius, Color.white, numParticles);
	}

	public static void Emit(ParticleEmitter _emitter, Vector3 _position, Quaternion _rotation, float _radius)
	{
		int numParticles = Mathf.RoundToInt(Random.Range(_emitter.minEmission, _emitter.maxEmission));
		Emit(_emitter, _position, _rotation, _radius, Color.white, numParticles);
	}

	public static void Emit(ParticleEmitter _emitter, Transform _transform, float _radius, Color _colour)
	{
		int numParticles = Mathf.RoundToInt(Random.Range(_emitter.minEmission, _emitter.maxEmission));
		Emit(_emitter, _transform, _radius, _colour, numParticles);
	}

	public static void Emit(ParticleEmitter _emitter, Vector3 _position, Quaternion _rotation, float _radius, Color _colour)
	{
		int numParticles = Mathf.RoundToInt(Random.Range(_emitter.minEmission, _emitter.maxEmission));
		Emit(_emitter, _position, _rotation, _radius, _colour, numParticles);
	}

	public static void Emit(ParticleEmitter _emitter, Transform _transform, float _radius, Color _colour, int _numParticles)
	{
		Emit(_emitter, _transform.position, _transform.rotation, _radius, _colour, _numParticles);
	}

	public static void Emit(ParticleEmitter _emitter, Vector3 _position, Quaternion _rotation, float _radius, Color _colour, int _numParticles)
	{
		for (int i = 0; i < _numParticles; i++)
		{
			Vector3 vector = _position;
			vector += Random.insideUnitSphere * _radius;
			Vector3 vector2 = _emitter.worldVelocity;
			vector2 += _rotation * _emitter.localVelocity;
			vector2 += Vector3.Scale(Random.insideUnitSphere, _emitter.rndVelocity);
			float size = Random.Range(_emitter.minSize, _emitter.maxSize);
			float energy = Random.Range(_emitter.minEnergy, _emitter.maxEnergy);
			if (!_emitter.useWorldSpace)
			{
				vector = _emitter.transform.InverseTransformPoint(vector);
				vector2 = _emitter.transform.InverseTransformDirection(vector2);
			}
			_emitter.Emit(vector, vector2, size, energy, _colour);
		}
	}

	public static void Emit(ParticleEmitter _emitter, Vector3 _position, Vector3 _velocity)
	{
		int num = Mathf.RoundToInt(Random.Range(_emitter.minEmission, _emitter.maxEmission));
		for (int i = 0; i < num; i++)
		{
			Vector3 velocity = _velocity;
			velocity += Vector3.Scale(Random.insideUnitSphere, _emitter.rndVelocity);
			float size = Random.Range(_emitter.minSize, _emitter.maxSize);
			float energy = Random.Range(_emitter.minEnergy, _emitter.maxEnergy);
			_emitter.Emit(_position, velocity, size, energy, Color.white);
		}
	}
}
