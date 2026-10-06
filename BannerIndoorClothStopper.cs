using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Moonforged.BannerCollection
{
    public class BannerIndoorClothStopper : MonoBehaviour
    {
        private const float RoofCheckInterval = 0.25f;

        private readonly List<Behaviour> clothComponents = new();
        private readonly List<bool> originalEnabledStates = new();

        private static Type clothType;
        private static MethodInfo isUnderRoofMethod;

        private float roofCheckTimer;
        private bool roofStateInitialized;
        private bool wasUnderRoof;

        private void Awake()
        {
            CacheReflection();
            CacheClothComponents();
        }

        private void OnEnable()
        {
            roofCheckTimer = 0f;
            roofStateInitialized = false;
        }

        private void OnDisable()
        {
            RestoreOriginalClothState();
        }

        private void OnDestroy()
        {
            RestoreOriginalClothState();
        }

        private void Update()
        {
            roofCheckTimer -= Time.deltaTime;
            if (roofCheckTimer > 0f)
                return;

            roofCheckTimer = RoofCheckInterval;

            if (clothComponents.Count == 0)
            {
                CacheClothComponents();
                if (clothComponents.Count == 0)
                    return;
            }

            bool underRoof = IsUnderRoof();
            if (roofStateInitialized && underRoof == wasUnderRoof)
                return;

            roofStateInitialized = true;
            wasUnderRoof = underRoof;
            SetClothSimulation(underRoof);
        }

        private static void CacheReflection()
        {
            if (clothType == null)
                clothType = FindRuntimeType("UnityEngine.Cloth");

            if (isUnderRoofMethod == null)
            {
                Type coverType = FindRuntimeType("Cover");
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
        }

        private static Type FindRuntimeType(string fullName)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type type = assembly.GetType(fullName, false);
                if (type != null)
                    return type;
            }

            return null;
        }

        private void CacheClothComponents()
        {
            clothComponents.Clear();
            originalEnabledStates.Clear();

            if (clothType == null)
                CacheReflection();

            if (clothType == null)
                return;

            Component[] found = GetComponentsInChildren(clothType, true);
            foreach (Component component in found)
            {
                if (component is Behaviour cloth)
                {
                    clothComponents.Add(cloth);
                    originalEnabledStates.Add(cloth.enabled);
                }
            }
        }

        private bool IsUnderRoof()
        {
            if (isUnderRoofMethod == null)
            {
                CacheReflection();
                if (isUnderRoofMethod == null)
                    return false;
            }

            foreach (Behaviour cloth in clothComponents)
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

        private void SetClothSimulation(bool underRoof)
        {
            for (int i = 0; i < clothComponents.Count; i++)
            {
                Behaviour cloth = clothComponents[i];
                if (cloth == null || i >= originalEnabledStates.Count)
                    continue;

                bool originallyEnabled = originalEnabledStates[i];
                cloth.enabled = underRoof ? false : originallyEnabled;
            }
        }

        private void RestoreOriginalClothState()
        {
            for (int i = 0; i < clothComponents.Count && i < originalEnabledStates.Count; i++)
            {
                Behaviour cloth = clothComponents[i];
                if (cloth != null)
                    cloth.enabled = originalEnabledStates[i];
            }
        }
    }
}
