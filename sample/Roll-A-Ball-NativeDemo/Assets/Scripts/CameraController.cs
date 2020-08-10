// Copyright (C) 2015 Google, Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Collections;
using UnityEngine;
using adivery;
using System.Security.Cryptography.X509Certificates;

/// <summary>
/// Camera controller.
/// </summary>
public class CameraController : MonoBehaviour
{
    public GameObject Ball;

    private Vector3 offset;

    public void Start()
    {
        this.offset = transform.position - this.Ball.transform.position;
        Adivery.configure("dee1366c-edbd-4f95-b3b8-cde1233eee35");
        Adivery.setLoggingEnabled(true);

        Adivery.requestBannerAd("5f2c4c86-a6ec-4735-9a44-f881fe40789f", Adivery.MEDIUM_RECTANGLE, Adivery.GRAVITY_TOP, new BannerAdCallback());

        Adivery.requestInterstitalAd("de5db046-765d-478f-bb2e-30dc2eaf3f51", new MyInterstitialAdCallback());
    }

    public void LateUpdate()
    {
        this.transform.position = this.Ball.transform.position + this.offset;
    }
}
