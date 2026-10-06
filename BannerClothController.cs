using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Moonforged.BannerCollection
{

    public class BannerClothController : MonoBehaviour
    {
        private const float UpdateInterval = 0.25f;

        private readonly List<Component> clothComponents = new();
        private readonly List<Component> globalWindComponents = new();
        private readonly Dictionary<Component, float> originalWindMultipliers = new();

        private static Type clothType;
        private static Type coverType;
        private static Type globalWindType;

        private static PropertyInfo externalAccelerationProperty;
        private static PropertyInfo randomAccelerationProperty;
        private static MethodInfo isUnderRoofMethod;
        private static MethodInfo updateWindMethod;
        private static FieldInfo windMultiplierField;

        private float updateTimer;
        private bool roofStateInitialized;
        private bool wasUnderRoof;

        private void Awake()
        {
            CacheReflection();
            FindClothComponents();
            FindGlobalWindComponents();
            UpdateRoofState();
        }

        private void OnEnable()
        {
            updateTimer = 0f;
            roofStateInitialized = false;
        }

        private void OnDisable()
        {
            RestoreOriginalWind();
        }

        private void OnDestroy()
        {
            RestoreOriginalWind();
        }

        private void Update()
        {
            updateTimer -= Time.deltaTime;
            if (updateTimer > 0f)
                return;

            updateTimer = UpdateInterval;
            UpdateRoofState();
        }

        private static void CacheReflection()
        {
            if (clothType == null)
            {
                clothType = Type.GetType("UnityEngine.Cloth, UnityEngine.ClothModule");
                if (clothType != null)
                {
                    externalAccelerationProperty = clothType.GetProperty("externalAcceleration");
                    randomAccelerationProperty = clothType.GetProperty("randomAcceleration");
                }
            }

            if (coverType == null)
            {
                coverType = Type.GetType("Cover, assembly_utils") ?? Type.GetType("Cover, assembly_valheim");
                if (coverType != null)
                {
                    isUnderRoofMethod = coverType.GetMethod(
                        "IsUnderRoof",
                        BindingFlags.Public | BindingFlags.Static,
                        null,
                        new[] { typeof(Vector3) },
                        null
                    );
                }
            }

            if (globalWindType == null)
            {
                globalWindType = Type.GetType("GlobalWind, assembly_valheim");
                if (globalWindType != null)
                {
                    windMultiplierField = globalWindType.GetField("m_multiplier", BindingFlags.Public | BindingFlags.Instance);
                    updateWindMethod = globalWindType.GetMethod("UpdateWind", BindingFlags.NonPublic | BindingFlags.Instance);
                }
            }
        }

        private void FindClothComponents()
        {
            clothComponents.Clear();

            if (clothType == null)
                return;

            Component[] found = GetComponentsInChildren(clothType, true);
            foreach (Component component in found)
            {
                if (component != null)
                    clothComponents.Add(component);
            }
        }

        private void FindGlobalWindComponents()
        {
            globalWindComponents.Clear();
            originalWindMultipliers.Clear();

            if (globalWindType == null)
                return;

            Component[] found = GetComponentsInChildren(globalWindType, true);
            foreach (Component component in found)
            {
                if (component == null)
                    continue;

                globalWindComponents.Add(component);

                if (windMultiplierField != null)
                {
                    object value = windMultiplierField.GetValue(component);
                    if (value is float multiplier)
                        originalWindMultipliers[component] = multiplier;
                }
            }
        }

        private void UpdateRoofState()
        {
            if (clothComponents.Count == 0)
            {
                FindClothComponents();

                if (clothComponents.Count == 0)
                    return;
            }

            if (globalWindComponents.Count == 0)
                FindGlobalWindComponents();

            bool underRoof = IsBannerUnderRoof();

            if (!roofStateInitialized || underRoof != wasUnderRoof)
            {
                roofStateInitialized = true;
                wasUnderRoof = underRoof;

                if (underRoof)
                    StopWindUnderRoof();
                else
                    RestoreOriginalWind();
            }

            if (underRoof)
                ZeroClothWind();
        }

        private bool IsBannerUnderRoof()
        {
            if (isUnderRoofMethod == null)
                return false;

            foreach (Component cloth in clothComponents)
            {
                if (cloth == null)
                    continue;

                Vector3 checkPoint = cloth.transform.position;
                Renderer renderer = cloth.GetComponent<Renderer>();
                if (renderer != null)
                    checkPoint = renderer.bounds.center;

                try
                {
                    object result = isUnderRoofMethod.Invoke(null, new object[] { checkPoint });
                    if (result is bool underRoof && underRoof)
                        return true;
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        private void StopWindUnderRoof()
        {
            foreach (Component globalWind in globalWindComponents)
            {
                if (globalWind == null || windMultiplierField == null)
                    continue;

                windMultiplierField.SetValue(globalWind, 0f);
                updateWindMethod?.Invoke(globalWind, null);
            }

            ZeroClothWind();
        }

        private void ZeroClothWind()
        {
            foreach (Component cloth in clothComponents)
            {
                if (cloth == null)
                    continue;

                externalAccelerationProperty?.SetValue(cloth, Vector3.zero, null);
                randomAccelerationProperty?.SetValue(cloth, Vector3.zero, null);
            }
        }

        private void RestoreOriginalWind()
        {
            foreach (Component globalWind in globalWindComponents)
            {
                if (globalWind == null || windMultiplierField == null)
                    continue;

                if (originalWindMultipliers.TryGetValue(globalWind, out float originalMultiplier))
                {
                    windMultiplierField.SetValue(globalWind, originalMultiplier);
                    updateWindMethod?.Invoke(globalWind, null);
                }
            }
        }
    }
}