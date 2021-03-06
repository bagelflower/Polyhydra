﻿using System;
using System.Collections.Generic;
using System.IO;
using Conway;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;


[Serializable]
public class PolyPreset {

	public const string PresetFileNamePrefix = "PolyPreset-";

	public string Name;

	[JsonConverter(typeof(StringEnumConverter))] public PolyHydra.ShapeTypes ShapeType;
	[JsonConverter(typeof(StringEnumConverter))] public PolyHydra.PolyTypeCategories PolyTypeCategory;
	[JsonConverter(typeof(StringEnumConverter))] public PolyTypes PolyType;
	[JsonConverter(typeof(StringEnumConverter))] public PolyHydra.JohnsonPolyTypes JohnsonPolyType;
	[JsonConverter(typeof(StringEnumConverter))] public PolyHydra.OtherPolyTypes OtherPolyType;
	[JsonConverter(typeof(StringEnumConverter))] public PolyHydra.GridTypes GridType;
	[JsonConverter(typeof(StringEnumConverter))] public PolyHydra.GridShapes GridShape;
	public bool BypassOps;
	public bool TwoSided;
	public int PrismP;
	public int PrismQ;
	public string AppearancePresetName;
	
	[Serializable]
	public struct Op {
		[JsonConverter(typeof(StringEnumConverter))] public PolyHydra.Ops OpType;
		[JsonConverter(typeof(StringEnumConverter))] public ConwayPoly.FaceSelections FaceSelections;
		public float Amount;
		public float AnimatedAmount; // Not needed for presets but needed for cache key generation
		public bool Randomize;
		public bool Disabled;
		public bool Animate;
		public float AnimationRate;
		public float AnimationAmount;
	}
	
	public Op[] Ops;
		
	public void CreateFromPoly(string presetName, PolyHydra poly)
	{
		Name = presetName;
		AppearancePresetName = poly.APresetName;
		ShapeType = poly.ShapeType;
		PolyType = poly.UniformPolyType;
		PolyTypeCategory = poly.UniformPolyTypeCategory;
		JohnsonPolyType = poly.JohnsonPolyType;
		OtherPolyType = poly.OtherPolyType;
		GridType = poly.GridType;
		GridShape = poly.GridShape;
		BypassOps = poly.BypassOps;
		PrismP = poly.PrismP;
		PrismQ = poly.PrismQ;
		TwoSided = poly.TwoSided;
		Ops = new Op[poly.ConwayOperators.Count];
		
		for (var index = 0; index < poly.ConwayOperators.Count; index++)
		{
			var polyOp = poly.ConwayOperators[index];
			var op = new Op
			{
				OpType = polyOp.opType,
				FaceSelections = polyOp.faceSelections,
				Amount = polyOp.amount,
				AnimatedAmount = polyOp.animatedAmount,
				Randomize = polyOp.randomize,
				Disabled = polyOp.disabled,
				Animate = polyOp.animate,
				AnimationRate = polyOp.animationRate,
				AnimationAmount = polyOp.animationAmount,
			};
			Ops[index] = op;
		}
	}

	public void ApplyToPoly(PolyHydra poly)
	{
		poly.ShapeType = ShapeType;
		poly.UniformPolyTypeCategory = PolyTypeCategory;
		poly.UniformPolyType = PolyType;
		poly.JohnsonPolyType = JohnsonPolyType;
		poly.OtherPolyType = OtherPolyType;
		poly.BypassOps = BypassOps;
		poly.TwoSided = TwoSided;
		poly.ConwayOperators = new List<PolyHydra.ConwayOperator>();
		poly.GridType = GridType;
		poly.GridShape = GridShape;
		poly.PrismP = PrismP;
		poly.PrismQ = PrismQ;
		poly.PresetName = Name;

		for (var index = 0; index < Ops.Length; index++)
		{
			var presetOp = Ops[index];
			var op = new PolyHydra.ConwayOperator
			{
				opType = presetOp.OpType,
				faceSelections = presetOp.FaceSelections,
				amount = presetOp.Amount,
				animatedAmount = presetOp.AnimatedAmount,
				randomize = presetOp.Randomize,
				disabled = presetOp.Disabled,
				animate = presetOp.Animate,
				animationRate = presetOp.AnimationRate,
				animationAmount = presetOp.AnimationAmount,
			};
			poly.ConwayOperators.Add(op);
		}
	}

	public void ApplyToPoly(PolyHydra poly, AppearancePresets aPresets, bool loadMatchingAppearance)
	{
		ApplyToPoly(poly);
		if (loadMatchingAppearance)
		{
			aPresets.ApplyPresetToPoly(AppearancePresetName);
		}
	}

	public void Save()
	{
		var fileName = Path.Combine(Application.persistentDataPath, PresetFileNamePrefix + Name + ".json");
        var polyJson = JsonConvert.SerializeObject(this, Formatting.Indented);
        File.WriteAllText(fileName, polyJson);
	}
}
