/* Copyright 2020 The MathWorks, Inc. */

#ifndef MATLABTYPESINTERFACE_HPP
#define MATLABTYPESINTERFACE_HPP

#include "MatlabDataArray.hpp"

// Use C++ Engine API if flag is defined. Use C++ Shared Library by default.
#ifdef ENGINE_APP
#include "MatlabEngine.hpp"
typedef matlab::engine::MATLABEngine MATLABControllerType;
#else
#include "MatlabCppSharedLib.hpp"
typedef matlab::cpplib::MATLABLibrary MATLABControllerType;
#endif

template<typename ControllerType>
class MATLABObject {
  public:
    operator matlab::data::Array() {
        return m_object;
    }

    MATLABObject() {}

    MATLABObject(std::shared_ptr<ControllerType> matlabPtr, std::u16string clsName) : m_matlabPtr(matlabPtr) {
        m_object = MATLABCallDefaultConstructor(clsName);
    }

    MATLABObject(std::shared_ptr<ControllerType> matlabPtr, matlab::data::Array obj) : m_matlabPtr(matlabPtr), m_object(obj) {}

  private:
    class ConvertToComplex {
      public:
        typedef matlab::data::Array result_type;

        /// If the array already contains complex numbers, do nothing
        template <typename T>
        result_type operator()(const matlab::data::TypedArray<std::complex<T>>& arr) {
            return arr;
        }

        /// Returns a converted copy of the array which contains complex numbers instead of reals
        template <typename T,
                  typename std::enable_if<std::is_arithmetic<T>::value &&
                                          !std::is_same<T, bool>::value>::type* = nullptr>
        result_type operator()(const matlab::data::TypedArray<T>& arr) {
            matlab::data::ArrayFactory f;
            matlab::data::TypedArray<std::complex<T>> retVal =
                f.createArray<std::complex<T>>(arr.getDimensions());
            auto it = retVal.begin();
            for (const auto& elem : arr) {
                *it = std::complex<T>(elem, 0);
                ++it;
            }
            return std::move(retVal);
        }

        /// Throws an exception if type cannot be converted to std::complex
        template <typename T>
        result_type operator()(const T& arr) {
            throw std::logic_error("Cannot convert this type to complex!");
        }
    };

  protected:
    std::shared_ptr<ControllerType> m_matlabPtr = nullptr;
    matlab::data::Array m_object = matlab::data::Array();

    /// Return the underlying MATLAB object
    matlab::data::Array MATLABGetObject() { return m_object; }

    // Methods for setting and getting MATLAB properties

   /**
    *  @brief Gets the specified non-complex scalar MATLAB property belonging to this object
    *  @param propName - The name of the property in MATLAB
    *  @return The returned value of the scalar property
    *  @exception InvalidArrayTypeException if type is not supported by TypedArray
    *  @exception - Exceptions can occur when calling into MATLAB. Check Doc for C++ Shared Library or C++ Engine API.
    */
    template<typename T>
    inline T MATLABGetScalarProperty(std::u16string propName)
    {
        matlab::data::Array value = MATLABGetArrayProperty(propName);
        return value[0];
    }

   /**
    *  @brief Gets the specified scalar MATLAB property belonging to this object. Handles gets that might return as complex.
    *  @param propName - The name of the property in MATLAB
    *  @return The returned value of the complex scalar property
    *  @exception InvalidArrayTypeException if type is not supported by TypedArray
    *  @exception - Exceptions can occur when calling into MATLAB. Check Doc for C++ Shared Library or C++ Engine API.
    */
    template<typename T>
    inline std::complex<T> MATLABGetComplexScalarProperty(std::u16string propName)
    {
        matlab::data::Array mdaVal = MATLABGetArrayProperty(propName);
        ConvertToComplex v;
        matlab::data::Array convertedVal = matlab::data::apply_visitor(mdaVal, v);
        matlab::data::TypedArray<std::complex<T>> value(convertedVal);
        return value[0];
    }


   /**
    *  @brief Sets the specified scalar MATLAB property belonging to this object
    *  @param propName - The name of the property in MATLAB
    *  @param value - The value to set the MATLAB property with
    *  @exception - InvalidArrayTypeException if type is not supported by create scalar
    *  @exception - Exceptions can occur when calling into MATLAB. Check Doc for C++ Shared Library or C++ Engine API.
    */
    template<typename T>
    inline void MATLABSetScalarProperty(std::u16string propName, T value)
    {
        matlab::data::ArrayFactory arrayFactory;
        MATLABSetArrayProperty(propName, arrayFactory.createScalar<T>(value));
    }

   /**
    *  @brief Gets the specified 1-D array MATLAB property belonging to this object
    *  @param propName - The name of the property in MATLAB
    *  @return the returned vector containing the property data
    *  @exception InvalidArrayTypeException if type is not supported by TypedArray
    *  @exception - Exceptions can occur when calling into MATLAB. Check Doc for C++ Shared Library or C++ Engine API.
    */
    template<typename T>
    inline std::vector<T> MATLABGetVectorProperty(std::u16string propName)
    {
        matlab::data::TypedArray<T> arr = MATLABGetArrayProperty(propName);
        std::vector<T> value(arr.cbegin(), arr.cend());
        return value;
    }


   /**
    *  @brief Sets the specified 1-D array MATLAB property belonging to this object
    *  @param propName - The name of the property in MATLAB
    *  @param value - The vector to set the MATLAB array property with
    *  @exception InvalidArrayTypeException if type is not supported by createArray
    *  @exception - Exceptions can occur when calling into MATLAB. Check Doc for C++ Shared Library or C++ Engine API.
    */
    template<typename T>
    inline void MATLABSetVectorProperty(std::u16string propName, std::vector<T> value)
    {
        matlab::data::ArrayFactory arrayFactory;
        MATLABSetArrayProperty(propName,
        arrayFactory.createArray<typename std::vector<T>::const_iterator, T>({ value.size(), size_t(1) }, value.cbegin(), value.cend()));
    }

    // Methods for calling the underlying MATLAB object's methods

   /**
    *  @brief Used for calling a method of this MATLAB object which has no outputs
    *  @param fcn - The method of this MATLAB object to call using feval
    *  @param args - The arguments to the method
    *  @exception - Exceptions can occur when calling into MATLAB. Check Doc for C++ Shared Library or C++ Engine API.
    */
    inline void MATLABCallNoOutputMethod(std::u16string fcn, std::vector<matlab::data::Array> args)
    {
        m_matlabPtr->feval(fcn, 0, args);
    }

   /**
    *  @brief Used for calling a method of this MATLAB object which has one output
    *  @param fcn - The method of this MATLAB object to call using feval
    *  @param args - The arguments to the method
    *  @return The 1 returned value that should result from calling the method in MATLAB
    *  @exception - Exceptions can occur when calling into MATLAB. Check Doc for C++ Shared Library or C++ Engine API.
    */
    inline matlab::data::Array MATLABCallOneOutputMethod(std::u16string fcn, std::vector<matlab::data::Array> args)
    {
        return m_matlabPtr->feval(fcn, args);
    }

   /**
    *  @brief Used for calling a method of this MATLAB object which has more than 1 output
    *  @param fcn - The method of this MATLAB object to call using feval
    *  @param args - The arguments to the method
    *  @return The returned values that should result from calling the method in MATLAB
    *  @exception - Exceptions can occur when calling into MATLAB. Check Doc for C++ Shared Library or C++ Engine API.
    */
    inline std::vector<matlab::data::Array> MATLABCallMultiOutputMethod(std::u16string fcn, size_t nOut,
		                                                                std::vector<matlab::data::Array> args)
    {
        return m_matlabPtr->feval(fcn, nOut, args);
    }

private:

   /**
    *  @brief Initializes the underlying MATLAB object by creating it in MATLAB via it's default constructor
    *  @param clsName - The class name of the object
    *  @return The shared copy of the MATLAB object, as a matlab::data::Array
    *  @exception - Exceptions can occur when calling into MATLAB. Check Doc for C++ Shared Library or C++ Engine API.
    */
    inline matlab::data::Array MATLABCallDefaultConstructor(std::u16string clsName)
    {
        std::vector<matlab::data::Array> args;
        matlab::data::Array obj = m_matlabPtr->feval(clsName, args);
        return obj;
    }

   /**
    *  @brief Gets the specified MATLAB property of the MATLAB object
    *  @param propName - The property to get
    *  @return The shared copy of the MATLAB property, as a matlab::data::Array
    *  @exception - Exceptions can occur when calling into MATLAB. Check Doc for C++ Shared Library or C++ Engine API.
    */
    inline matlab::data::Array MATLABGetArrayProperty(std::u16string propName)
    {
        matlab::data::ArrayFactory arrayFactory;

        matlab::data::StructArray subsArray = arrayFactory.createStructArray({ 1, 1 }, { "type", "subs" });
        subsArray[0]["type"] = arrayFactory.createCharArray(".");
        subsArray[0]["subs"] = arrayFactory.createCharArray(propName);
        return m_matlabPtr->feval(u"subsref", { m_object, subsArray });
    }

   /**
    *  @brief Sets the specified MATLAB property of the MATLAB object
    *  @param propName - The property to Set
    *  @param value - The array value to set the property to
    *  @exception - Exceptions can occur when calling into MATLAB. Check Doc for C++ Shared Library or C++ Engine API.
    */
    inline void MATLABSetArrayProperty(std::u16string propName, matlab::data::Array value)
    {
        matlab::data::ArrayFactory arrayFactory;

        matlab::data::StructArray subsArray = arrayFactory.createStructArray({ 1, 1 }, { "type", "subs" });
        subsArray[0]["type"] = arrayFactory.createCharArray(".");
        subsArray[0]["subs"] = arrayFactory.createCharArray(propName);
        m_object = m_matlabPtr->feval(u"subsasgn", { m_object, subsArray, value });
    }

};

// Property getter/setter edge-cases covered via template specialization

/// Gets a char
template<>
template<>
inline char16_t MATLABObject<MATLABControllerType>::MATLABGetScalarProperty(std::u16string propName)
{
    matlab::data::TypedArray<char16_t> charResult = MATLABObject<MATLABControllerType>::MATLABGetArrayProperty(propName);
    return charResult[0];
}

/// Sets a char
template<>
template<>
inline void MATLABObject<MATLABControllerType>::MATLABSetScalarProperty(std::u16string propName, char16_t value)
{
    matlab::data::ArrayFactory arrayFactory;
    std::u16string str(1, value);
    MATLABObject<MATLABControllerType>::MATLABSetArrayProperty(propName, arrayFactory.createCharArray(str));
}

/// Gets a string
template<>
template<>
inline std::u16string MATLABObject<MATLABControllerType>::MATLABGetScalarProperty(std::u16string propName)
{
    matlab::data::Array result = MATLABObject<MATLABControllerType>::MATLABGetArrayProperty(propName);
    if (result.getType() == matlab::data::ArrayType::CHAR) {
        matlab::data::CharArray charResult(result);
        return charResult.toUTF16();
    }
    else {
        matlab::data::TypedArray<matlab::data::MATLABString> stringResult(result);
        matlab::data::MATLABString str = stringResult[0];
        if (str.has_value()) {
            return *str;
        }
        else {
            return u"";
        }
    }
}

/// Sets a string
template<>
template<>
inline void MATLABObject<MATLABControllerType>::MATLABSetScalarProperty(std::u16string propName, std::u16string value)
{
    matlab::data::ArrayFactory arrayFactory;
    MATLABObject<MATLABControllerType>::MATLABSetArrayProperty(propName, arrayFactory.createCharArray(value));
}

/// Gets a vector of strings
template<>
template<>
inline std::vector<std::u16string> MATLABObject<MATLABControllerType>::MATLABGetVectorProperty(std::u16string propName)
{
    matlab::data::Array strArray = MATLABGetArrayProperty(propName);
    size_t numel = strArray.getNumberOfElements();
    std::vector<std::u16string> strVector(numel);
    matlab::data::TypedArray<matlab::data::MATLABString> tempStringArray(strArray);

    // Copy each MATLAB string to the std::vector, if element has a value
    for (size_t k = 0; k < numel; k++) {
        matlab::data::MATLABString tempString = tempStringArray[k];
        if(tempString.has_value()) {
            strVector.at(k) = *tempString;
        }
        else {
            strVector.at(k) = u"";
        }
    }
    return strVector;
}

/// Sets a vector of strings
template<>
template<>
inline void MATLABObject<MATLABControllerType>::MATLABSetVectorProperty(std::u16string propName, std::vector<std::u16string> value)
{
    matlab::data::ArrayFactory arrayFactory;
    size_t numel = value.size();
    matlab::data::StringArray strArray = arrayFactory.createArray<matlab::data::MATLABString>({1,numel});
    std::copy(value.begin(), value.end(), strArray.begin());
    MATLABObject<MATLABControllerType>::MATLABSetArrayProperty(propName, strArray);
}

/// Gets a matlab::data::Array
template<>
template<>
inline matlab::data::Array MATLABObject<MATLABControllerType>::MATLABGetScalarProperty(std::u16string propName)
{
    return MATLABObject<MATLABControllerType>::MATLABGetArrayProperty(propName);
}

/// Sets a matlab::data::Array
template<>
template<>
inline void MATLABObject<MATLABControllerType>::MATLABSetScalarProperty(std::u16string propName, matlab::data::Array value)
{
    MATLABObject<MATLABControllerType>::MATLABSetArrayProperty(propName, value);
}

#endif  //MATLABTYPESINTERFACE_HPP