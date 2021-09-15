/**
 * Published header for C++ MEX
 *
 * Copyright 2017-2018 The MathWorks, Inc.
 */

#if defined(_MSC_VER)
# pragma once
#endif
#if defined(__GNUC__) && (__GNUC__ > 3 || (__GNUC__ == 3 && __GNUC_MINOR__ > 3))
# pragma once
#endif

#ifndef mex_hpp
#define mex_hpp
#endif


#ifndef __MEX_CPP_PUBLISHED_API_HPP__
#define __MEX_CPP_PUBLISHED_API_HPP__

#define __MEX_CPP_API__


#ifndef EXTERN_C
#  ifdef __cplusplus
#    define EXTERN_C extern "C"
#  else
#    define EXTERN_C extern
#  endif
#endif

#if defined(BUILDING_LIBMEX)
#  include "mex/mex_typedefs.hpp"
#  include "mex/libmwmex_util.hpp"
#else
#  ifndef LIBMWMEX_API
#      define LIBMWMEX_API
#  endif

#  ifndef LIBMWMEX_API_EXTERN_C
#     define LIBMWMEX_API_EXTERN_C EXTERN_C LIBMWMEX_API
#  endif
#endif

#ifdef _WIN32
#       define DLL_EXPORT_SYM __declspec(dllexport)
#       define SUPPORTS_PRAGMA_ONCE
#elif __GNUC__ >= 4
#       define DLL_EXPORT_SYM __attribute__ ((visibility("default")))
#       define SUPPORTS_PRAGMA_ONCE
#else
#       define DLL_EXPORT_SYM
#endif

#ifdef DLL_EXPORT_SYM
# define MEXFUNCTION_LINKAGE EXTERN_C DLL_EXPORT_SYM
#else
# ifdef MW_NEEDS_VERSION_H
#  include "version.h"
#  define MEXFUNCTION_LINKAGE EXTERN_C DLL_EXPORT_SYM
# else
#  define MEXFUNCTION_LINKAGE EXTERN_C
# endif
#endif


#if defined(_WIN32 )
#define NOEXCEPT throw()
#else
#define NOEXCEPT noexcept
#endif

#if defined(_MSC_VER) && _MSC_VER==1800
#define NOEXCEPT_FALSE	throw(...)
#else
#define NOEXCEPT_FALSE noexcept(false)
#endif

#include "cppmex/mexMatlabEngine.hpp"
#include "cppmex/mexFunction.hpp"
#include "cppmex/mexException.hpp"
#include "cppmex/mexFuture.hpp"
#include "cppmex/mexTaskReference.hpp"

#endif //__MEX_CPP_PUBLISHED_API_HPP__
