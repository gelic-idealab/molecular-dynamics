// University of Illinois/NCSA
// Open Source License
// http://otm.illinois.edu/disclose-protect/illinois-open-source-license

// Copyright (c) 2020 Grainger Engineering Library Information Center.  All rights reserved.

// Developed by: IDEA Lab
//               Grainger Engineering Library Information Center - University of Illinois Urbana-Champaign
//               https://library.illinois.edu/enx

// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the "Software"), to deal with
// the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
// of the Software, and to permit persons to whom the Software is furnished to
// do so, subject to the following conditions:
// * Redistributions of source code must retain the above copyright notice,
//   this list of conditions and the following disclaimers.
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimers in the documentation
//   and/or other materials provided with the distribution.
// * Neither the names of IDEA Lab, Grainger Engineering Library Information Center,
//   nor the names of its contributors may be used to endorse or promote products
//   derived from this Software without specific prior written permission.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.  IN NO EVENT SHALL THE
// CONTRIBUTORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS WITH THE
// SOFTWARE.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Komodo.AssetImport;
using Komodo.Runtime;

public class ModelItemHack : MonoBehaviour
{
    public Transform modelListParent; //TOOD(Brandon): somehow move away from public assignment. (ModelImportInitializer.list is private.)

    private Transform buttonListParent; //TODO(Brandon): get this directly from the menu.

    public GameObject buttonTemplate; //TODO(Brandon): somehow move away from public assignment.

    public void Start ()
    {
        StartCoroutine(StartWhenReady());
    }

    public IEnumerator StartWhenReady ()
    {
        yield return new WaitUntil(() => UIManager.Instance.IsReady() && GameStateManager.Instance.isAssetImportFinished);

        if (!buttonListParent)
        {
            buttonListParent = transform;
        }

        NetworkedGameObject defaultNetObject = default;

        ModelImportInitializer.Instance.networkedGameObjects.Add(defaultNetObject); //TODO -- don't call this directly

        int thisIndex = ModelImportInitializer.Instance.networkedGameObjects.Count - 1;

        var data = new ModelDataTemplate.ModelImportData();

        data.isWholeObject = true;

        data.scale = 1;

        var setupFlags = new ModelImportSettings();

        setupFlags.doSetUpColliders = false;

        var newParent = ModelImportPostProcessor.SetUpGameObject(thisIndex, data, gameObject, setupFlags);

        gameObject.transform.SetParent(newParent.transform, true);

        // Copy collider from this object to the parent object, since it needs to be on the same object as the netObject component.

        var childBoxCol = GetComponent<BoxCollider>();

        if (childBoxCol)
        {
            var parentBoxCol = newParent.AddComponent<BoxCollider>();

            parentBoxCol.center = transform.TransformPoint(childBoxCol.center);

            Vector3 size = childBoxCol.size;

            size.Scale(transform.localScale);

            parentBoxCol.size = size;

            Destroy(childBoxCol);
        }

        var childSphCol = GetComponent<SphereCollider>();

        if (childSphCol)
        {
            var parentSphCol = newParent.AddComponent<SphereCollider>();

            parentSphCol.center = transform.TransformPoint(childSphCol.center);

            parentSphCol.radius = childSphCol.radius * transform.localScale.x;

            Destroy(childSphCol);
        }

        // Create button

        buttonListParent = UIManager.Instance.menu.GetComponentInChildren<ModelButtonList>().transformToPlaceButtonUnder; //TODO(Brandon): make this less coupled.

        GameObject item = Instantiate(buttonTemplate, buttonListParent);

        if (item.TryGetComponent(out ModelItem modelItem))
        {
            string name = gameObject.name;

            if (name == null)
            {
                Debug.LogError($"gameObject.name was null. Proceeding anyways.");

                name = "null";
            }

            modelItem.Initialize(thisIndex, name);
        }
        else
        {
            throw new MissingComponentException("modelItem on GameObject (from ModelButtonTemplate)");
        }
    }
}