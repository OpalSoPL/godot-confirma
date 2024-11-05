using System;
using System.Collections.Generic;
using Confirma.Attributes;
using Confirma.Classes;
using Confirma.Extensions;
using Confirma.Fuzz;

namespace Confirma.Tests;

[TestClass]
[Parallelizable]
public class FuzzGeneratorTest
{
    #region NextValue
    [TestCase]
    public void NextValue_Int_ReturnsIntValue()
    {
        FuzzGenerator fuzzValue = new(
            typeof(int),
            name: null,
            1,
            10,
            EDistributionType.Uniform,
            seed: null
        );

        _ = fuzzValue.NextValue().ConfirmInstanceOf<int>();
    }

    [TestCase]
    public void NextValue_Double_ReturnsDoubleValue()
    {
        FuzzGenerator fuzzValue = new(
            typeof(double),
            name: null,
            1,
            10,
            EDistributionType.Uniform,
            seed: null
        );

        _ = fuzzValue.NextValue().ConfirmInstanceOf<double>();
    }

    [TestCase]
    public void NextValue_Float_ReturnsFloatValue()
    {
        FuzzGenerator fuzzValue = new(
            typeof(float),
            name: null,
            1,
            10,
            EDistributionType.Uniform,
            seed: null
        );

        _ = fuzzValue.NextValue().ConfirmInstanceOf<float>();
    }

    [TestCase]
    public void NextValue_String_ReturnsStringValue()
    {
        FuzzGenerator fuzzValue = new(
            typeof(string),
            name: null,
            1,
            10,
            EDistributionType.Uniform,
            seed: null
        );

        _ = fuzzValue.NextValue().ConfirmInstanceOf<string>();
    }

    [TestCase]
    public void NextValue_Bool_ReturnsBoolValue()
    {
        FuzzGenerator fuzzValue = new(
            typeof(bool),
            name: null,
            1,
            10,
            EDistributionType.Uniform,
            seed: null
        );

        _ = fuzzValue.NextValue().ConfirmInstanceOf<bool>();
    }

    [TestCase]
    public void NextValue_UnsupportedDataType_ThrowsArgumentException()
    {
        FuzzGenerator fuzzValue = new(
            typeof(List<int>),
            name: null,
            1,
            10,
            EDistributionType.Uniform,
            seed: null
        );

        _ = Confirm.Throws<ArgumentException>(() => fuzzValue.NextValue());
    }

    [TestCase]
    public void NextValue_Seed_ReturnsSameValue()
    {
        FuzzGenerator fuzzValue1 = new(
            typeof(int),
            name: null,
            1,
            10,
            EDistributionType.Uniform,
            123
        );
        FuzzGenerator fuzzValue2 = new(
            typeof(int),
            name: null,
            1,
            10,
            EDistributionType.Uniform,
            123
        );

        _ = fuzzValue1.NextValue().ConfirmEqual(fuzzValue2.NextValue());
    }

    [TestCase]
    public void NextValue_outSeed_ReturnsDifferentValue()
    {
        FuzzGenerator fuzzValue1 = new(
            typeof(int),
            name: null,
            1,
            10,
            EDistributionType.Uniform,
            seed: null
        );
        FuzzGenerator fuzzValue2 = new(
            typeof(int),
            name: null,
            1,
            10,
            EDistributionType.Uniform,
            seed: null
        );

        _ = fuzzValue1.NextValue().ConfirmNotEqual(fuzzValue2.NextValue());
    }
    #endregion NextValue

    #region NextInt
    [TestCase]
    public void NextInt_Uniform_ReturnsValueWithinRange()
    {
        FuzzGenerator fuzzValue = new(
            typeof(int),
            name: null,
            1,
            10,
            EDistributionType.Uniform,
            seed: null
        );

        _ = ((int)fuzzValue.NextValue()).ConfirmInRange(1, 10);
    }

    [TestCase]
    public void NextInt_Gaussian_ReturnsValueWithinRange()
    {
        FuzzGenerator fuzzValue = new(
            typeof(int),
            name: null,
            1,
            10,
            EDistributionType.Gaussian,
            seed: null
        );

        _ = ((int)fuzzValue.NextValue()).ConfirmInRange(-20, 20);
    }

    [TestCase]
    public void NextInt_Exponential_ReturnsValueWithinRange()
    {
        FuzzGenerator fuzzValue = new(
            typeof(int),
            name: null,
            0.001f,
            10,
            EDistributionType.Exponential,
            seed: null
        );

        _ = ((int)fuzzValue.NextValue()).ConfirmGreaterThan(0);
    }

    [TestCase]
    public void NextInt_Poisson_ReturnsValueWithinRange()
    {
        FuzzGenerator fuzzValue = new(
            typeof(int),
            name: null,
            5,
            10,
            EDistributionType.Poisson,
            seed: null
        );

        _ = ((int)fuzzValue.NextValue()).ConfirmInRange(1, 10);
    }
    #endregion NextInt

    #region NextDouble
    [TestCase]
    public void NextDouble_Uniform_ReturnsValueWithinRange()
    {
        FuzzGenerator fuzzValue = new(
            typeof(double),
            name: null,
            1,
            10,
            EDistributionType.Uniform,
            seed: null
        );

        _ = ((double)fuzzValue.NextValue()).ConfirmInRange(1, 10);
    }

    [TestCase]
    public void NextDouble_Gaussian_ReturnsValueWithinRange()
    {
        FuzzGenerator fuzzValue = new(
            typeof(double),
            name: null,
            1,
            10,
            EDistributionType.Gaussian,
            seed: null
            );

        _ = ((double)fuzzValue.NextValue()).ConfirmInRange(-20, 20);
    }

    [TestCase]
    public void NextDouble_Exponential_ReturnsValueWithinRange()
    {
        FuzzGenerator fuzzValue = new(
            typeof(double),
            name: null,
            0.001f,
            10,
            EDistributionType.Exponential,
            seed: null
        );

        _ = ((double)fuzzValue.NextValue()).ConfirmGreaterThan(0);
    }

    [TestCase]
    public void NextDouble_Poisson_ReturnsValueWithinRange()
    {
        FuzzGenerator fuzzValue = new(
            typeof(double),
            name: null,
            5,
            10,
            EDistributionType.Poisson,
            seed: null
        );

        _ = ((double)fuzzValue.NextValue()).ConfirmInRange(1, 10);
    }
    #endregion NextDouble

    #region NextString
    [TestCase]
    public void NextString_Uniform_ReturnsStringValueWithinRange()
    {
        FuzzGenerator fuzzValue = new(
            typeof(string),
            name: null,
            1,
            10,
            EDistributionType.Uniform,
            seed: null
        );

        _ = ((string)fuzzValue.NextValue()).Length.ConfirmInRange(1, 10);
    }
    #endregion NextString
}