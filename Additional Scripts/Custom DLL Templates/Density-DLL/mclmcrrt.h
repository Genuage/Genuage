/*
 * @(#)mclmcrrt.h
 *
 *				apiproxy.published
 *				libmat_proxy.cpp
 *				libmwmclbase_proxy.cpp
 *				libmwmclmcr_proxy.cpp
 *				libmx_proxy.cpp
 */

#if defined(_MSC_VER)
# pragma once
#endif
#if defined(__GNUC__) && (__GNUC__ > 3 || (__GNUC__ == 3 && __GNUC_MINOR__ > 3))
# pragma once
#endif

#ifndef mclmcrrt_h
#define mclmcrrt_h


/*
 * Copyright 1984-2003 The MathWorks, Inc.
 * All Rights Reserved.
 */



/* Copyright 2003-2006 The MathWorks, Inc. */

/* Only define EXTERN_C if it hasn't been defined already. This allows
 * individual modules to have more control over managing their exports.
 */
#ifndef EXTERN_C

#ifdef __cplusplus
  #define EXTERN_C extern "C"
#else
  #define EXTERN_C extern
#endif

#endif



#ifdef __LCC__
/* Must undefine EXTERN_C here (and redefine it later) because LCC's version
 * of windows.h has its own definition of EXTERN_C.
 */
#undef EXTERN_C
#endif

#ifdef _WIN32
#include <windows.h>
#endif

#ifdef __LCC__
#undef EXTERN_C
#define EXTERN_C extern
#endif

#ifndef _WIN32
typedef const struct _GUID *REFCLSID, *REFGUID;
typedef long HRESULT;
#endif



#  if defined( linux ) || defined( __linux ) || defined( __linux__ )
/* stdint.h must be included before sys/types.h or loadlibrary will fail.
 * Because matrix.h includes stdlib.h, which includes sys/types.h, stdint.h
 * must be included before any include of matrix.h (On Linux systems.)
 */
#include <stdint.h> 
#endif

/*#ifdef matrix_h
#error "mclmcrrt.h must be included before matrix.h. (Since mclmcrrt.h includes matrix.h, additional inclusion is redundant.)"
#endif */
#include "matrix.h"

#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))

#undef mclmcrInitialize2
#define mclmcrInitialize2 mclmcrInitialize2_proxy


#undef mclmcrInitialize
#define mclmcrInitialize mclmcrInitialize_proxy


#undef mclInitializeApplication
#if !defined(TARGET_API_VERSION) || TARGET_API_VERSION >= 800
#define mclInitializeApplication mclInitializeApplication_800_proxy
#else
#define mclInitializeApplication mclInitializeApplication_730_proxy
#endif   /* !defined(TARGET_API_VERSION) || TARGET_API_VERSION >= 800 */


#undef mclDisplayStartMessage
#define mclDisplayStartMessage mclDisplayStartMessage_proxy

#endif /* !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__))) */

typedef void * MCREventHandlerArg;
typedef void (*MCREventHandlerFcn)(MCREventHandlerArg);
typedef enum
{   MCRStartEvent,
    MCRCompleteEvent
} mcrInitializationEventType;
typedef void * MCREventData;

EXTERN_C void mclDisplayStartMessage_proxy(mcrInitializationEventType eventType,MCREventHandlerFcn fcn,MCREventHandlerArg arg,MCREventData eventData);


#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclGetComponentInfo
#define mclGetComponentInfo mclGetComponentInfo_proxy
#endif 


EXTERN_C HRESULT mclGetComponentInfo_proxy(const char* lpszComponent, 
                                                      int nMajorRev, 
                                                      int nMinorRev, int nInfo, 
                                                      int nType, 
                                                      void** info);


#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclGetLIBIDInfo
#define mclGetLIBIDInfo mclGetLIBIDInfo_proxy
#endif 


EXTERN_C HRESULT mclGetLIBIDInfo_proxy(const char* lpszLIBID, 
                                                  int nMajorRev, int nMinorRev, 
                                                  int nInfo, void** info);


#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclRegisterServer
#define mclRegisterServer mclRegisterServer_proxy
#endif 


EXTERN_C HRESULT mclRegisterServer_proxy(const char* szModuleName,     
                                                    REFCLSID clsid,               
                                                    REFGUID libid,                
                                                    unsigned short wMajorRev,     
                                                    unsigned short wMinorRev,     
                                                    const char* szFriendlyName,   
                                                    const char* szVerIndProgID,   
                                                    const char* szProgID,         
                                                    const char* szThreadingModel);


#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclGUIDFromString
#define mclGUIDFromString mclGUIDFromString_proxy
#endif 


EXTERN_C int mclGUIDFromString_proxy(const char* lpszGUID, struct _GUID* pguid);


#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclUnRegisterMatLabCOMComponent
#define mclUnRegisterMatLabCOMComponent mclUnRegisterMatLabCOMComponent_proxy
#endif 


EXTERN_C HRESULT mclUnRegisterMatLabCOMComponent_proxy(REFCLSID clsid,            
                                       const char* szVerIndProgID, 
                                       const char* szProgID);


#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclRegisterMatLabXLComponent
#define mclRegisterMatLabXLComponent mclRegisterMatLabXLComponent_proxy
#endif 


EXTERN_C HRESULT mclRegisterMatLabXLComponent_proxy(const char* szModuleName,    
                                    REFCLSID clsid,               
                                    REFGUID libid,                
                                    unsigned short wMajorRev,     
                                    unsigned short wMinorRev,     
                                    const char* szFriendlyName,   
                                    const char* szVerIndProgID,   
                                    const char* szProgID);


#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclGUIDtochar
#define mclGUIDtochar mclGUIDtochar_proxy
#endif 


EXTERN_C void mclGUIDtochar_proxy(REFGUID guid, char* szGUID, int length);


#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclUnregisterServer
#define mclUnregisterServer mclUnregisterServer_proxy
#endif 


EXTERN_C HRESULT mclUnregisterServer_proxy(REFCLSID clsid,             
                           const char* szVerIndProgID, 
                           const char* szProgID);


#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclCLSIDtochar
#define mclCLSIDtochar mclCLSIDtochar_proxy
#endif 


EXTERN_C void mclCLSIDtochar_proxy(REFCLSID clsid, char* szCLSID, int length);


#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclFreeComponentInfo
#define mclFreeComponentInfo mclFreeComponentInfo_proxy
#endif 


EXTERN_C void mclFreeComponentInfo_proxy(void** info);


#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclUnRegisterMatLabXLComponent
#define mclUnRegisterMatLabXLComponent mclUnRegisterMatLabXLComponent_proxy
#endif 


EXTERN_C HRESULT mclUnRegisterMatLabXLComponent_proxy(REFCLSID clsid,             
                                      const char* szVerIndProgID, 
                                      const char* szProgID);


#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclRegisterMatLabCOMComponent
#define mclRegisterMatLabCOMComponent mclRegisterMatLabCOMComponent_proxy
#endif 


EXTERN_C HRESULT mclRegisterMatLabCOMComponent_proxy(const char* szModuleName,     
                                     REFCLSID clsid,               
                                     REFGUID libid,                
                                     unsigned short wMajorRev,     
                                     unsigned short wMinorRev,     
                                     const char* szFriendlyName,   
                                     const char* szVerIndProgID,   
                                     const char* szProgID);

#ifndef MW_CALL_CONV
#  ifdef _WIN32 
#      define MW_CALL_CONV __cdecl
#  else
#      define MW_CALL_CONV 
#  endif
#endif

#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef matOpen
#define matOpen matOpen_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef matClose
#define matClose matClose_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef matGetErrno
#define matGetErrno matGetErrno_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef matGetFp
#define matGetFp matGetFp_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef matPutVariable
#define matPutVariable matPutVariable_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef matPutVariableAsGlobal
#define matPutVariableAsGlobal matPutVariableAsGlobal_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef matGetVariable
#define matGetVariable matGetVariable_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef matGetNextVariable
#define matGetNextVariable matGetNextVariable_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef matGetNextVariableInfo
#define matGetNextVariableInfo matGetNextVariableInfo_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef matGetVariableInfo
#define matGetVariableInfo matGetVariableInfo_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef matDeleteVariable
#define matDeleteVariable matDeleteVariable_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef matGetDir
#define matGetDir matGetDir_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef matOpen_800
#define matOpen_800 matOpen_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef matClose_800
#define matClose_800 matClose_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef matGetErrno_800
#define matGetErrno_800 matGetErrno_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef matGetFp_800
#define matGetFp_800 matGetFp_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef matPutVariable_800
#define matPutVariable_800 matPutVariable_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef matPutVariableAsGlobal_800
#define matPutVariableAsGlobal_800 matPutVariableAsGlobal_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef matGetVariable_800
#define matGetVariable_800 matGetVariable_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef matGetNextVariable_800
#define matGetNextVariable_800 matGetNextVariable_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef matGetNextVariableInfo_800
#define matGetNextVariableInfo_800 matGetNextVariableInfo_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef matGetVariableInfo_800
#define matGetVariableInfo_800 matGetVariableInfo_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef matDeleteVariable_800
#define matDeleteVariable_800 matDeleteVariable_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef matGetDir_800
#define matGetDir_800 matGetDir_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */




/*#ifdef mat_h
#error "mclmcrrt.h must be included before mat.h. (Since mclmcrrt.h includes mat.h, additional inclusion is redundant.)"
#endif */
#define LIBMWMAT_API_EXTERN_C EXTERN_C
#include "mat.h"

/* Proxies for functions in mat.h */

#if TARGET_API_VERSION < 800
EXTERN_C
MATFile * matOpen_proxy(const char *a0, const char *a1);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
matError matClose_proxy(MATFile *a0);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
matError matGetErrno_proxy(MATFile *a0);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
FILE * matGetFp_proxy(MATFile *a0);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
matError matPutVariable_proxy(MATFile *a0, const char *a1, 
    const mxArray *a2);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
matError matPutVariableAsGlobal_proxy(MATFile *a0, const char *a1, 
    const mxArray *a2);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
mxArray * matGetVariable_proxy(MATFile *a0, const char *a1);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
mxArray * matGetNextVariable_proxy(MATFile *a0, const char **a1);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
mxArray * matGetNextVariableInfo_proxy(MATFile *a0, const char **a1);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
mxArray * matGetVariableInfo_proxy(MATFile *a0, const char *a1);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
matError matDeleteVariable_proxy(MATFile *a0, const char *a1);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
char ** matGetDir_proxy(MATFile *a0, int *a1);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
MATFile * matOpen_800_proxy(const char *a0, const char *a1);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
matError matClose_800_proxy(MATFile *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
matError matGetErrno_800_proxy(MATFile *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
FILE * matGetFp_800_proxy(MATFile *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
matError matPutVariable_800_proxy(MATFile *a0, const char *a1, 
    const mxArray *a2);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
matError matPutVariableAsGlobal_800_proxy(MATFile *a0, const char *a1, 
    const mxArray *a2);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
mxArray * matGetVariable_800_proxy(MATFile *a0, const char *a1);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
mxArray * matGetNextVariable_800_proxy(MATFile *a0, const char **a1);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
mxArray * matGetNextVariableInfo_800_proxy(MATFile *a0, const char **a1);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
mxArray * matGetVariableInfo_800_proxy(MATFile *a0, const char *a1);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
matError matDeleteVariable_800_proxy(MATFile *a0, const char *a1);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
char ** matGetDir_800_proxy(MATFile *a0, int *a1);
#endif



#ifndef MW_CALL_CONV
#  ifdef _WIN32 
#      define MW_CALL_CONV __cdecl
#  else
#      define MW_CALL_CONV 
#  endif
#endif

/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclSetLastErrIdAndMsg
#define mclSetLastErrIdAndMsg mclSetLastErrIdAndMsg_proxy
#endif



/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclGetLastErrorMessage
#define mclGetLastErrorMessage mclGetLastErrorMessage_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclGetStackTrace
#define mclGetStackTrace mclGetStackTrace_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclFreeStackTrace
#define mclFreeStackTrace mclFreeStackTrace_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclAcquireMutex
#define mclAcquireMutex mclAcquireMutex_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclReleaseMutex
#define mclReleaseMutex mclReleaseMutex_proxy
#endif



/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclIsMCRInitialized
#define mclIsMCRInitialized mclIsMCRInitialized_proxy
#endif



/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclIsJVMEnabled
#define mclIsJVMEnabled mclIsJVMEnabled_proxy
#endif



/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclGetLogFileName
#define mclGetLogFileName mclGetLogFileName_proxy
#endif



/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclIsNoDisplaySet
#define mclIsNoDisplaySet mclIsNoDisplaySet_proxy
#endif



/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclInitializeApplicationInternal
#define mclInitializeApplicationInternal mclInitializeApplicationInternal_proxy
#endif



/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclTerminateApplication
#define mclTerminateApplication mclTerminateApplication_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclIsMcc
#define mclIsMcc mclIsMcc_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef separatePathName
#define separatePathName separatePathName_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclFreeStrArray
#define mclFreeStrArray mclFreeStrArray_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclFreeArrayList
#define mclFreeArrayList mclFreeArrayList_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclCreateCellArrayFromArrayList
#define mclCreateCellArrayFromArrayList mclCreateCellArrayFromArrayList_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclCreateSharedCopy
#define mclCreateSharedCopy mclCreateSharedCopy_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclCreateEmptyArray
#define mclCreateEmptyArray mclCreateEmptyArray_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclCreateSimpleFunctionHandle
#define mclCreateSimpleFunctionHandle mclCreateSimpleFunctionHandle_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclMxSerialize
#define mclMxSerialize mclMxSerialize_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclMxDeserialize
#define mclMxDeserialize mclMxDeserialize_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclSetInterleavedCompatibility
#define mclSetInterleavedCompatibility mclSetInterleavedCompatibility_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclIsInterleavedCompatibility
#define mclIsInterleavedCompatibility mclIsInterleavedCompatibility_proxy
#endif



/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclRunMain
#define mclRunMain mclRunMain_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclCFRunLoopRun
#define mclCFRunLoopRun mclCFRunLoopRun_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclCFRunLoopStop
#define mclCFRunLoopStop mclCFRunLoopStop_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclIsCFRunLoopReady
#define mclIsCFRunLoopReady mclIsCFRunLoopReady_proxy
#endif




/*#ifdef mclbase_h
#error "mclmcrrt.h must be included before mclbase.h. (Since mclmcrrt.h includes mclbase.h, additional inclusion is redundant.)"
#endif */
#define LIBMWMCLBASE_API_EXTERN_C EXTERN_C
#include "mclbase.h"

/* Proxies for functions in mclbase.h */

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
void mclSetLastErrIdAndMsg_proxy(const char *a0, const char *a1);

EXTERN_C
const char * mclGetLastErrorMessage_proxy();

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclGetStackTrace_proxy(char ***a0);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclFreeStackTrace_proxy(char ***a0, int a1);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
void mclAcquireMutex_proxy();

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
void mclReleaseMutex_proxy();

EXTERN_C
bool mclIsMCRInitialized_proxy();

EXTERN_C
bool mclIsJVMEnabled_proxy();

EXTERN_C
const char * mclGetLogFileName_proxy();

EXTERN_C
bool mclIsNoDisplaySet_proxy();

EXTERN_C
bool mclInitializeApplicationInternal_proxy(const char **a0, size_t a1);

EXTERN_C
bool mclTerminateApplication_proxy();

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
bool mclIsMcc_proxy();

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
void separatePathName_proxy(const char *a0, char *a1, size_t a2);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
bool mclFreeStrArray_proxy(char **a0, size_t a1);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
void mclFreeArrayList_proxy(int a0, mxArray **a1);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
mxArray * mclCreateCellArrayFromArrayList_proxy(int a0, mxArray **a1);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
mxArray * mclCreateSharedCopy_proxy(mxArray *a0);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
mxArray * mclCreateEmptyArray_proxy();

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
mxArray * mclCreateSimpleFunctionHandle_proxy(mxFunctionPtr a0);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
mxArray * mclMxSerialize_proxy(mxArray *a0);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
mxArray * mclMxDeserialize_proxy(const void *a0, size_t a1);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
void mclSetInterleavedCompatibility_proxy(bool a0);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
bool mclIsInterleavedCompatibility_proxy();

EXTERN_C
int mclRunMain_proxy(mclMainFcnType a0, int a1, const char **a2);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
void mclCFRunLoopRun_proxy();

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
void mclCFRunLoopStop_proxy();

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
bool mclIsCFRunLoopReady_proxy();



#ifndef MW_CALL_CONV
#  ifdef _WIN32 
#      define MW_CALL_CONV __cdecl
#  else
#      define MW_CALL_CONV 
#  endif
#endif

/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclGetStreamFromArraySrc
#define mclGetStreamFromArraySrc mclGetStreamFromArraySrc_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclDestroyStream
#define mclDestroyStream mclDestroyStream_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclGetEmbeddedCtfStream
#define mclGetEmbeddedCtfStream mclGetEmbeddedCtfStream_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclInitializeComponentInstanceNonEmbeddedStandalone
#define mclInitializeComponentInstanceNonEmbeddedStandalone mclInitializeComponentInstanceNonEmbeddedStandalone_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclInitializeInstanceWithoutComponent
#define mclInitializeInstanceWithoutComponent mclInitializeInstanceWithoutComponent_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclInitializeComponentInstanceCtfFileToCache
#define mclInitializeComponentInstanceCtfFileToCache mclInitializeComponentInstanceCtfFileToCache_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclInitializeComponentInstanceEmbedded
#define mclInitializeComponentInstanceEmbedded mclInitializeComponentInstanceEmbedded_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclInitializeComponentInstanceWithCallbk
#define mclInitializeComponentInstanceWithCallbk mclInitializeComponentInstanceWithCallbk_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclInitializeComponentInstanceFromExtractedComponent
#define mclInitializeComponentInstanceFromExtractedComponent mclInitializeComponentInstanceFromExtractedComponent_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclInitializeComponentInstanceFromExtractedLocation
#define mclInitializeComponentInstanceFromExtractedLocation mclInitializeComponentInstanceFromExtractedLocation_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclGetDotNetComponentType
#define mclGetDotNetComponentType mclGetDotNetComponentType_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclGetMCCTargetType
#define mclGetMCCTargetType mclGetMCCTargetType_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef getStandaloneFileName
#define getStandaloneFileName getStandaloneFileName_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclStandaloneGenericMain
#define mclStandaloneGenericMain mclStandaloneGenericMain_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclStandaloneCtfxMain
#define mclStandaloneCtfxMain mclStandaloneCtfxMain_proxy
#endif



/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclWaitForFiguresToDie
#define mclWaitForFiguresToDie mclWaitForFiguresToDie_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclcppGetLastError
#define mclcppGetLastError mclcppGetLastError_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclcppCreateError
#define mclcppCreateError mclcppCreateError_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclcppSetLastError
#define mclcppSetLastError mclcppSetLastError_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclcppErrorCheck
#define mclcppErrorCheck mclcppErrorCheck_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclcppGetLastErrorMessage
#define mclcppGetLastErrorMessage mclcppGetLastErrorMessage_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclCreateCharBuffer
#define mclCreateCharBuffer mclCreateCharBuffer_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclGetEps
#define mclGetEps mclGetEps_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclGetInf
#define mclGetInf mclGetInf_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclGetNaN
#define mclGetNaN mclGetNaN_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclIsFinite
#define mclIsFinite mclIsFinite_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclIsInf
#define mclIsInf mclIsInf_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclIsNaN
#define mclIsNaN mclIsNaN_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclIsIdentical
#define mclIsIdentical mclIsIdentical_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclGetEmptyArray
#define mclGetEmptyArray mclGetEmptyArray_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclGetMatrix
#define mclGetMatrix mclGetMatrix_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclGetArray
#define mclGetArray mclGetArray_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclGetNumericMatrix
#define mclGetNumericMatrix mclGetNumericMatrix_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclGetNumericArray
#define mclGetNumericArray mclGetNumericArray_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclGetScalarDouble
#define mclGetScalarDouble mclGetScalarDouble_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclGetScalarSingle
#define mclGetScalarSingle mclGetScalarSingle_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclGetScalarInt8
#define mclGetScalarInt8 mclGetScalarInt8_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclGetScalarUint8
#define mclGetScalarUint8 mclGetScalarUint8_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclGetScalarInt16
#define mclGetScalarInt16 mclGetScalarInt16_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclGetScalarUint16
#define mclGetScalarUint16 mclGetScalarUint16_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclGetScalarInt32
#define mclGetScalarInt32 mclGetScalarInt32_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclGetScalarUint32
#define mclGetScalarUint32 mclGetScalarUint32_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclGetScalarInt64
#define mclGetScalarInt64 mclGetScalarInt64_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclGetScalarUint64
#define mclGetScalarUint64 mclGetScalarUint64_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclGetCharMatrix
#define mclGetCharMatrix mclGetCharMatrix_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclGetCharArray
#define mclGetCharArray mclGetCharArray_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclGetScalarChar
#define mclGetScalarChar mclGetScalarChar_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclGetString
#define mclGetString mclGetString_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclGetCharMatrixFromStrings
#define mclGetCharMatrixFromStrings mclGetCharMatrixFromStrings_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclIsMatlabString
#define mclIsMatlabString mclIsMatlabString_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclIsMissingStringElement
#define mclIsMissingStringElement mclIsMissingStringElement_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclCreateMatlabString
#define mclCreateMatlabString mclCreateMatlabString_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclCreateMatlabStringArray
#define mclCreateMatlabStringArray mclCreateMatlabStringArray_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclMatlabStringGetElement
#define mclMatlabStringGetElement mclMatlabStringGetElement_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclMatlabStringSetElement
#define mclMatlabStringSetElement mclMatlabStringSetElement_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclMatlabStringGetData
#define mclMatlabStringGetData mclMatlabStringGetData_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclMatlabStringSetData
#define mclMatlabStringSetData mclMatlabStringSetData_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclMatlabStringGetNumberOfDimensions
#define mclMatlabStringGetNumberOfDimensions mclMatlabStringGetNumberOfDimensions_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclMatlabStringGetDimensions
#define mclMatlabStringGetDimensions mclMatlabStringGetDimensions_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclMatlabStringGetNumberOfElements
#define mclMatlabStringGetNumberOfElements mclMatlabStringGetNumberOfElements_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclGetMatlabString
#define mclGetMatlabString mclGetMatlabString_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclGetMatlabStringArray
#define mclGetMatlabStringArray mclGetMatlabStringArray_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclGetLogicalMatrix
#define mclGetLogicalMatrix mclGetLogicalMatrix_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclGetLogicalArray
#define mclGetLogicalArray mclGetLogicalArray_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclGetScalarLogical
#define mclGetScalarLogical mclGetScalarLogical_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclGetCellMatrix
#define mclGetCellMatrix mclGetCellMatrix_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclGetCellArray
#define mclGetCellArray mclGetCellArray_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclGetStructMatrix
#define mclGetStructMatrix mclGetStructMatrix_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclGetStructArray
#define mclGetStructArray mclGetStructArray_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclGetNumericSparse
#define mclGetNumericSparse mclGetNumericSparse_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclGetNumericSparseInferRowsCols
#define mclGetNumericSparseInferRowsCols mclGetNumericSparseInferRowsCols_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclGetLogicalSparse
#define mclGetLogicalSparse mclGetLogicalSparse_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclGetLogicalSparseInferRowsCols
#define mclGetLogicalSparseInferRowsCols mclGetLogicalSparseInferRowsCols_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclDeserializeArray
#define mclDeserializeArray mclDeserializeArray_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclcppGetArrayBuffer
#define mclcppGetArrayBuffer mclcppGetArrayBuffer_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclcppFeval
#define mclcppFeval mclcppFeval_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclcppArrayToString
#define mclcppArrayToString mclcppArrayToString_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclcppFreeString
#define mclcppFreeString mclcppFreeString_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclmxArray2ArrayHandle
#define mclmxArray2ArrayHandle mclmxArray2ArrayHandle_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclArrayHandle2mxArray
#define mclArrayHandle2mxArray mclArrayHandle2mxArray_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclMXArrayGetIndexArrays
#define mclMXArrayGetIndexArrays mclMXArrayGetIndexArrays_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclMXArrayGet
#define mclMXArrayGet mclMXArrayGet_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclMXArrayGetReal
#define mclMXArrayGetReal mclMXArrayGetReal_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclMXArrayGetImag
#define mclMXArrayGetImag mclMXArrayGetImag_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclMXArraySet
#define mclMXArraySet mclMXArraySet_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclMXArraySetReal
#define mclMXArraySetReal mclMXArraySetReal_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclMXArraySetImag
#define mclMXArraySetImag mclMXArraySetImag_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclMXArraySetLogical
#define mclMXArraySetLogical mclMXArraySetLogical_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclMxRefDestroyArray
#define mclMxRefDestroyArray mclMxRefDestroyArray_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclMxRefSerialize
#define mclMxRefSerialize mclMxRefSerialize_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclMxRefDeserialize
#define mclMxRefDeserialize mclMxRefDeserialize_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclMxRefMvmId
#define mclMxRefMvmId mclMxRefMvmId_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclHashNBytes
#define mclHashNBytes mclHashNBytes_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclCalcSingleSubscript
#define mclCalcSingleSubscript mclCalcSingleSubscript_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclCreateCharMatrixFromUTF16Strings
#define mclCreateCharMatrixFromUTF16Strings mclCreateCharMatrixFromUTF16Strings_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mcl2DCharArrayToUTF16Strings
#define mcl2DCharArrayToUTF16Strings mcl2DCharArrayToUTF16Strings_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclWrite
#define mclWrite mclWrite_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclAddCanonicalPathMacro
#define mclAddCanonicalPathMacro mclAddCanonicalPathMacro_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclFevalInternal
#define mclFevalInternal mclFevalInternal_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclGetMaxPathLen
#define mclGetMaxPathLen mclGetMaxPathLen_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclmcrInitializeInternal2
#define mclmcrInitializeInternal2 mclmcrInitializeInternal2_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclmcrInitializeInternal
#define mclmcrInitializeInternal mclmcrInitializeInternal_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef deleteWcsStackPointer_hPtr
#define deleteWcsStackPointer_hPtr deleteWcsStackPointer_hPtr_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef initializeWcsStackPointer
#define initializeWcsStackPointer initializeWcsStackPointer_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef deleteWcsStackPointer
#define deleteWcsStackPointer deleteWcsStackPointer_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef allocWcsStackPointer
#define allocWcsStackPointer allocWcsStackPointer_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mwMbstowcs
#define mwMbstowcs mwMbstowcs_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef utf16_to_lcp_n_fcn
#define utf16_to_lcp_n_fcn utf16_to_lcp_n_fcn_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef utf16_strlen_fcn
#define utf16_strlen_fcn utf16_strlen_fcn_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef utf16_strncpy_fcn
#define utf16_strncpy_fcn utf16_strncpy_fcn_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef utf16_strdup_fcn
#define utf16_strdup_fcn utf16_strdup_fcn_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclSetGlobal
#define mclSetGlobal mclSetGlobal_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclIsStandaloneMode
#define mclIsStandaloneMode mclIsStandaloneMode_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclImpersonationFeval
#define mclImpersonationFeval mclImpersonationFeval_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclGetGlobal
#define mclGetGlobal mclGetGlobal_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclGetID
#define mclGetID mclGetID_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclMain
#define mclMain mclMain_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclMlfVFevalInternal
#define mclMlfVFevalInternal mclMlfVFevalInternal_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclGetMCRVersion
#define mclGetMCRVersion mclGetMCRVersion_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclGetActiveID
#define mclGetActiveID mclGetActiveID_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclGetTempFileName
#define mclGetTempFileName mclGetTempFileName_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclTerminateInstance
#define mclTerminateInstance mclTerminateInstance_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef stopImpersonationOnMCRThread
#define stopImpersonationOnMCRThread stopImpersonationOnMCRThread_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclMxIsA
#define mclMxIsA mclMxIsA_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclMxIsRef
#define mclMxIsRef mclMxIsRef_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclMxRefIsA
#define mclMxRefIsA mclMxRefIsA_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclMxRefGetRefClassName
#define mclMxRefGetRefClassName mclMxRefGetRefClassName_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclMxRefGetProperty
#define mclMxRefGetProperty mclMxRefGetProperty_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclMxRefSetProperty
#define mclMxRefSetProperty mclMxRefSetProperty_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclMxReleaseRef
#define mclMxReleaseRef mclMxReleaseRef_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclMxRefLocalMvm
#define mclMxRefLocalMvm mclMxRefLocalMvm_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclMxDestroyArray
#define mclMxDestroyArray mclMxDestroyArray_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclNonDefaultAppDomainInUse
#define mclNonDefaultAppDomainInUse mclNonDefaultAppDomainInUse_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mclIsNonDefaultAppDomainInUse
#define mclIsNonDefaultAppDomainInUse mclIsNonDefaultAppDomainInUse_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef ref_count_obj_addref
#define ref_count_obj_addref ref_count_obj_addref_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef ref_count_obj_release
#define ref_count_obj_release ref_count_obj_release_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef char_buffer_size
#define char_buffer_size char_buffer_size_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef char_buffer_get_buffer
#define char_buffer_get_buffer char_buffer_get_buffer_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef char_buffer_set_buffer
#define char_buffer_set_buffer char_buffer_set_buffer_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef char_buffer_compare_to
#define char_buffer_compare_to char_buffer_compare_to_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_classID
#define array_ref_classID array_ref_classID_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_deep_copy
#define array_ref_deep_copy array_ref_deep_copy_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_detach
#define array_ref_detach array_ref_detach_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_shared_copy
#define array_ref_shared_copy array_ref_shared_copy_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_serialize
#define array_ref_serialize array_ref_serialize_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_element_size
#define array_ref_element_size array_ref_element_size_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_number_of_elements
#define array_ref_number_of_elements array_ref_number_of_elements_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_number_of_nonzeros
#define array_ref_number_of_nonzeros array_ref_number_of_nonzeros_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_maximum_nonzeros
#define array_ref_maximum_nonzeros array_ref_maximum_nonzeros_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_number_of_dimensions
#define array_ref_number_of_dimensions array_ref_number_of_dimensions_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_get_dimensions
#define array_ref_get_dimensions array_ref_get_dimensions_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_number_of_fields
#define array_ref_number_of_fields array_ref_number_of_fields_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_get_field_name
#define array_ref_get_field_name array_ref_get_field_name_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_is_empty
#define array_ref_is_empty array_ref_is_empty_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_is_sparse
#define array_ref_is_sparse array_ref_is_sparse_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_is_numeric
#define array_ref_is_numeric array_ref_is_numeric_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_is_complex
#define array_ref_is_complex array_ref_is_complex_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_is_matlab_string
#define array_ref_is_matlab_string array_ref_is_matlab_string_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_is_missing_string_element
#define array_ref_is_missing_string_element array_ref_is_missing_string_element_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_make_complex
#define array_ref_make_complex array_ref_make_complex_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_equals
#define array_ref_equals array_ref_equals_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_compare_to
#define array_ref_compare_to array_ref_compare_to_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_hash_code
#define array_ref_hash_code array_ref_hash_code_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_to_string
#define array_ref_to_string array_ref_to_string_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_row_index
#define array_ref_row_index array_ref_row_index_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_column_index
#define array_ref_column_index array_ref_column_index_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_get_int
#define array_ref_get_int array_ref_get_int_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_get_const_char
#define array_ref_get_const_char array_ref_get_const_char_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_getV_int
#define array_ref_getV_int array_ref_getV_int_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_getV_const_char
#define array_ref_getV_const_char array_ref_getV_const_char_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_set
#define array_ref_set array_ref_set_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_real
#define array_ref_real array_ref_real_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_imag
#define array_ref_imag array_ref_imag_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_get_numeric_mxDouble
#define array_ref_get_numeric_mxDouble array_ref_get_numeric_mxDouble_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_get_numeric_mxSingle
#define array_ref_get_numeric_mxSingle array_ref_get_numeric_mxSingle_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_get_numeric_mxInt8
#define array_ref_get_numeric_mxInt8 array_ref_get_numeric_mxInt8_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_get_numeric_mxUint8
#define array_ref_get_numeric_mxUint8 array_ref_get_numeric_mxUint8_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_get_numeric_mxInt16
#define array_ref_get_numeric_mxInt16 array_ref_get_numeric_mxInt16_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_get_numeric_mxUint16
#define array_ref_get_numeric_mxUint16 array_ref_get_numeric_mxUint16_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_get_numeric_mxInt32
#define array_ref_get_numeric_mxInt32 array_ref_get_numeric_mxInt32_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_get_numeric_mxUint32
#define array_ref_get_numeric_mxUint32 array_ref_get_numeric_mxUint32_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_get_numeric_mxInt64
#define array_ref_get_numeric_mxInt64 array_ref_get_numeric_mxInt64_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_get_numeric_mxUint64
#define array_ref_get_numeric_mxUint64 array_ref_get_numeric_mxUint64_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_get_char
#define array_ref_get_char array_ref_get_char_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_get_string_element
#define array_ref_get_string_element array_ref_get_string_element_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_get_matlab_string
#define array_ref_get_matlab_string array_ref_get_matlab_string_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_get_logical
#define array_ref_get_logical array_ref_get_logical_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_set_numeric_mxDouble
#define array_ref_set_numeric_mxDouble array_ref_set_numeric_mxDouble_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_set_numeric_mxSingle
#define array_ref_set_numeric_mxSingle array_ref_set_numeric_mxSingle_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_set_numeric_mxInt8
#define array_ref_set_numeric_mxInt8 array_ref_set_numeric_mxInt8_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_set_numeric_mxUint8
#define array_ref_set_numeric_mxUint8 array_ref_set_numeric_mxUint8_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_set_numeric_mxInt16
#define array_ref_set_numeric_mxInt16 array_ref_set_numeric_mxInt16_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_set_numeric_mxUint16
#define array_ref_set_numeric_mxUint16 array_ref_set_numeric_mxUint16_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_set_numeric_mxInt32
#define array_ref_set_numeric_mxInt32 array_ref_set_numeric_mxInt32_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_set_numeric_mxUint32
#define array_ref_set_numeric_mxUint32 array_ref_set_numeric_mxUint32_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_set_numeric_mxInt64
#define array_ref_set_numeric_mxInt64 array_ref_set_numeric_mxInt64_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_set_numeric_mxUint64
#define array_ref_set_numeric_mxUint64 array_ref_set_numeric_mxUint64_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_set_char
#define array_ref_set_char array_ref_set_char_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_set_string_element
#define array_ref_set_string_element array_ref_set_string_element_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_set_matlab_string
#define array_ref_set_matlab_string array_ref_set_matlab_string_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_ref_set_logical
#define array_ref_set_logical array_ref_set_logical_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_buffer_size
#define array_buffer_size array_buffer_size_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_buffer_get
#define array_buffer_get array_buffer_get_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_buffer_set
#define array_buffer_set array_buffer_set_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_buffer_add
#define array_buffer_add array_buffer_add_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_buffer_remove
#define array_buffer_remove array_buffer_remove_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_buffer_clear
#define array_buffer_clear array_buffer_clear_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_buffer_to_cell
#define array_buffer_to_cell array_buffer_to_cell_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef error_info_get_message
#define error_info_get_message error_info_get_message_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef error_info_get_stack_trace
#define error_info_get_stack_trace error_info_get_stack_trace_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_handle_classID
#define array_handle_classID array_handle_classID_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_handle_deep_copy
#define array_handle_deep_copy array_handle_deep_copy_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_handle_detach
#define array_handle_detach array_handle_detach_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_handle_shared_copy
#define array_handle_shared_copy array_handle_shared_copy_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_handle_serialize
#define array_handle_serialize array_handle_serialize_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_handle_element_size
#define array_handle_element_size array_handle_element_size_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_handle_number_of_elements
#define array_handle_number_of_elements array_handle_number_of_elements_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_handle_number_of_nonzeros
#define array_handle_number_of_nonzeros array_handle_number_of_nonzeros_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_handle_maximum_nonzeros
#define array_handle_maximum_nonzeros array_handle_maximum_nonzeros_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_handle_number_of_dimensions
#define array_handle_number_of_dimensions array_handle_number_of_dimensions_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_handle_get_dimensions
#define array_handle_get_dimensions array_handle_get_dimensions_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_handle_number_of_fields
#define array_handle_number_of_fields array_handle_number_of_fields_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_handle_get_field_name
#define array_handle_get_field_name array_handle_get_field_name_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_handle_is_empty
#define array_handle_is_empty array_handle_is_empty_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_handle_is_sparse
#define array_handle_is_sparse array_handle_is_sparse_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_handle_is_numeric
#define array_handle_is_numeric array_handle_is_numeric_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_handle_is_complex
#define array_handle_is_complex array_handle_is_complex_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_handle_make_complex
#define array_handle_make_complex array_handle_make_complex_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_handle_equals
#define array_handle_equals array_handle_equals_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_handle_compare_to
#define array_handle_compare_to array_handle_compare_to_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_handle_hash_code
#define array_handle_hash_code array_handle_hash_code_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_handle_to_string
#define array_handle_to_string array_handle_to_string_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_handle_row_index
#define array_handle_row_index array_handle_row_index_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_handle_column_index
#define array_handle_column_index array_handle_column_index_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_handle_get_int
#define array_handle_get_int array_handle_get_int_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_handle_get_const_char
#define array_handle_get_const_char array_handle_get_const_char_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_handle_getV_int
#define array_handle_getV_int array_handle_getV_int_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_handle_getV_const_char
#define array_handle_getV_const_char array_handle_getV_const_char_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_handle_set
#define array_handle_set array_handle_set_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_handle_real
#define array_handle_real array_handle_real_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_handle_imag
#define array_handle_imag array_handle_imag_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_handle_get_numeric_mxDouble
#define array_handle_get_numeric_mxDouble array_handle_get_numeric_mxDouble_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_handle_get_numeric_mxSingle
#define array_handle_get_numeric_mxSingle array_handle_get_numeric_mxSingle_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_handle_get_numeric_mxInt8
#define array_handle_get_numeric_mxInt8 array_handle_get_numeric_mxInt8_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_handle_get_numeric_mxUint8
#define array_handle_get_numeric_mxUint8 array_handle_get_numeric_mxUint8_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_handle_get_numeric_mxInt16
#define array_handle_get_numeric_mxInt16 array_handle_get_numeric_mxInt16_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_handle_get_numeric_mxUint16
#define array_handle_get_numeric_mxUint16 array_handle_get_numeric_mxUint16_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_handle_get_numeric_mxInt32
#define array_handle_get_numeric_mxInt32 array_handle_get_numeric_mxInt32_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_handle_get_numeric_mxUint32
#define array_handle_get_numeric_mxUint32 array_handle_get_numeric_mxUint32_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_handle_get_numeric_mxInt64
#define array_handle_get_numeric_mxInt64 array_handle_get_numeric_mxInt64_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_handle_get_numeric_mxUint64
#define array_handle_get_numeric_mxUint64 array_handle_get_numeric_mxUint64_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_handle_get_char
#define array_handle_get_char array_handle_get_char_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_handle_get_logical
#define array_handle_get_logical array_handle_get_logical_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_handle_set_numeric_mxDouble
#define array_handle_set_numeric_mxDouble array_handle_set_numeric_mxDouble_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_handle_set_numeric_mxSingle
#define array_handle_set_numeric_mxSingle array_handle_set_numeric_mxSingle_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_handle_set_numeric_mxInt8
#define array_handle_set_numeric_mxInt8 array_handle_set_numeric_mxInt8_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_handle_set_numeric_mxUint8
#define array_handle_set_numeric_mxUint8 array_handle_set_numeric_mxUint8_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_handle_set_numeric_mxInt16
#define array_handle_set_numeric_mxInt16 array_handle_set_numeric_mxInt16_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_handle_set_numeric_mxUint16
#define array_handle_set_numeric_mxUint16 array_handle_set_numeric_mxUint16_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_handle_set_numeric_mxInt32
#define array_handle_set_numeric_mxInt32 array_handle_set_numeric_mxInt32_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_handle_set_numeric_mxUint32
#define array_handle_set_numeric_mxUint32 array_handle_set_numeric_mxUint32_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_handle_set_numeric_mxInt64
#define array_handle_set_numeric_mxInt64 array_handle_set_numeric_mxInt64_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_handle_set_numeric_mxUint64
#define array_handle_set_numeric_mxUint64 array_handle_set_numeric_mxUint64_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_handle_set_char
#define array_handle_set_char array_handle_set_char_proxy
#endif



/**This function is for INTERNAL USE ONLY.**/
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef array_handle_set_logical
#define array_handle_set_logical array_handle_set_logical_proxy
#endif




/*#ifdef mclmcr_h
#error "mclmcrrt.h must be included before mclmcr.h. (Since mclmcrrt.h includes mclmcr.h, additional inclusion is redundant.)"
#endif */
#define LIBMWMCLMCR_API_EXTERN_C EXTERN_C
#include "mclmcr.h"

/* Proxies for functions in mclmcr.h */

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
mclCtfStream mclGetStreamFromArraySrc_proxy(char *a0, int a1);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
void mclDestroyStream_proxy(mclCtfStream a0);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
mclCtfStream mclGetEmbeddedCtfStream_proxy(void *a0);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
bool mclInitializeComponentInstanceNonEmbeddedStandalone_proxy(
    HMCRINSTANCE *a0, const char *a1, const char *a2, mccTargetType a3, 
    mclOutputHandlerFcn a4, mclOutputHandlerFcn a5);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
bool mclInitializeInstanceWithoutComponent_proxy(HMCRINSTANCE *a0, 
    const char **a1, size_t a2, mclOutputHandlerFcn a3, 
    mclOutputHandlerFcn a4);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
bool mclInitializeComponentInstanceCtfFileToCache_proxy(HMCRINSTANCE *a0, 
    mclOutputHandlerFcn a1, mclOutputHandlerFcn a2, const char *a3);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
bool mclInitializeComponentInstanceEmbedded_proxy(HMCRINSTANCE *a0, 
    mclOutputHandlerFcn a1, mclOutputHandlerFcn a2, mclCtfStream a3);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
bool mclInitializeComponentInstanceWithCallbk_proxy(HMCRINSTANCE *a0, 
    mclOutputHandlerFcn a1, mclOutputHandlerFcn a2, 
    mclReadCtfStreamFcn a3, size_t a4);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
bool mclInitializeComponentInstanceFromExtractedComponent_proxy(
    HMCRINSTANCE *a0, mclOutputHandlerFcn a1, mclOutputHandlerFcn a2, 
    const char *a3);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
bool mclInitializeComponentInstanceFromExtractedLocation_proxy(
    HMCRINSTANCE *a0, mclOutputHandlerFcn a1, mclOutputHandlerFcn a2, 
    const char *a3);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclGetDotNetComponentType_proxy();

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclGetMCCTargetType_proxy(bool a0);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
const char * getStandaloneFileName_proxy(const char *a0, const char *a1);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
bool mclStandaloneGenericMain_proxy(size_t a0, const char **a1, 
    const char *a2, bool a3, void *a4);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
bool mclStandaloneCtfxMain_proxy(size_t a0, const char **a1);

EXTERN_C
void mclWaitForFiguresToDie_proxy(HMCRINSTANCE a0);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclcppGetLastError_proxy(void **a0);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclcppCreateError_proxy(void **a0, const char *a1);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
void mclcppSetLastError_proxy(const char *a0);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclcppErrorCheck_proxy();

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
const char * mclcppGetLastErrorMessage_proxy();

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclCreateCharBuffer_proxy(void **a0, const char *a1);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
double mclGetEps_proxy();

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
double mclGetInf_proxy();

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
double mclGetNaN_proxy();

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
bool mclIsFinite_proxy(double a0);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
bool mclIsInf_proxy(double a0);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
bool mclIsNaN_proxy(double a0);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
bool mclIsIdentical_proxy(mxArray *a0, mxArray *a1);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclGetEmptyArray_proxy(void **a0, mxClassID a1);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclGetMatrix_proxy(void **a0, mwSize a1, mwSize a2, mxClassID a3, 
    mxComplexity a4);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclGetArray_proxy(void **a0, mwSize a1, const mwSize *a2, 
    mxClassID a3, mxComplexity a4);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclGetNumericMatrix_proxy(void **a0, mwSize a1, mwSize a2, 
    mxClassID a3, mxComplexity a4);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclGetNumericArray_proxy(void **a0, mwSize a1, const mwSize *a2, 
    mxClassID a3, mxComplexity a4);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclGetScalarDouble_proxy(void **a0, mxDouble a1, mxDouble a2, 
    mxComplexity a3);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclGetScalarSingle_proxy(void **a0, mxSingle a1, mxSingle a2, 
    mxComplexity a3);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclGetScalarInt8_proxy(void **a0, mxInt8 a1, mxInt8 a2, 
    mxComplexity a3);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclGetScalarUint8_proxy(void **a0, mxUint8 a1, mxUint8 a2, 
    mxComplexity a3);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclGetScalarInt16_proxy(void **a0, mxInt16 a1, mxInt16 a2, 
    mxComplexity a3);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclGetScalarUint16_proxy(void **a0, mxUint16 a1, mxUint16 a2, 
    mxComplexity a3);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclGetScalarInt32_proxy(void **a0, mxInt32 a1, mxInt32 a2, 
    mxComplexity a3);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclGetScalarUint32_proxy(void **a0, mxUint32 a1, mxUint32 a2, 
    mxComplexity a3);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclGetScalarInt64_proxy(void **a0, mxInt64 a1, mxInt64 a2, 
    mxComplexity a3);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclGetScalarUint64_proxy(void **a0, mxUint64 a1, mxUint64 a2, 
    mxComplexity a3);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclGetCharMatrix_proxy(void **a0, mwSize a1, mwSize a2);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclGetCharArray_proxy(void **a0, mwSize a1, const mwSize *a2);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclGetScalarChar_proxy(void **a0, mxChar a1);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclGetString_proxy(void **a0, const char *a1);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclGetCharMatrixFromStrings_proxy(void **a0, mwSize a1, 
    const char **a2);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
bool mclIsMatlabString_proxy(const mxArray *a0);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
bool mclIsMissingStringElement_proxy(const mxArray *a0, mwSize a1);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
mxArray * mclCreateMatlabString_proxy(mwSize a0, const mxChar **a1);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
mxArray * mclCreateMatlabStringArray_proxy(mwSize a0, const mwSize *a1);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclMatlabStringGetElement_proxy(const mxArray *a0, mwSize a1, 
    const mxChar **a2, mwSize *a3);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclMatlabStringSetElement_proxy(mxArray *a0, mwSize a1, 
    const mxChar *a2);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclMatlabStringGetData_proxy(const mxArray *a0, const mxChar **a1, 
    mwSize *a2, mwSize *a3);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclMatlabStringSetData_proxy(mxArray *a0, const mxChar **a1, 
    mwSize a2);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclMatlabStringGetNumberOfDimensions_proxy(const mxArray *a0, 
    mwSize *a1);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclMatlabStringGetDimensions_proxy(const mxArray *a0, 
    const mwSize **a1);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclMatlabStringGetNumberOfElements_proxy(const mxArray *a0, 
    mwSize *a1);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclGetMatlabString_proxy(void **a0, mwSize a1, const mxChar **a2);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclGetMatlabStringArray_proxy(void **a0, mwSize a1, const mwSize *a2);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclGetLogicalMatrix_proxy(void **a0, mwSize a1, mwSize a2);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclGetLogicalArray_proxy(void **a0, mwSize a1, const mwSize *a2);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclGetScalarLogical_proxy(void **a0, mxLogical a1);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclGetCellMatrix_proxy(void **a0, mwSize a1, mwSize a2);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclGetCellArray_proxy(void **a0, mwSize a1, const mwSize *a2);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclGetStructMatrix_proxy(void **a0, mwSize a1, mwSize a2, int a3, 
    const char **a4);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclGetStructArray_proxy(void **a0, mwSize a1, const mwSize *a2, 
    int a3, const char **a4);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclGetNumericSparse_proxy(void **a0, mwSize a1, const mwSize *a2, 
    mwSize a3, const mwSize *a4, mwSize a5, const void *a6, 
    const void *a7, mwSize a8, mwSize a9, mwSize a10, mxClassID a11, 
    mxComplexity a12);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclGetNumericSparseInferRowsCols_proxy(void **a0, mwSize a1, 
    const mwSize *a2, mwSize a3, const mwSize *a4, mwSize a5, 
    const void *a6, const void *a7, mwSize a8, mxClassID a9, 
    mxComplexity a10);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclGetLogicalSparse_proxy(void **a0, mwSize a1, const mwIndex *a2, 
    mwSize a3, const mwIndex *a4, mwSize a5, const mxLogical *a6, 
    mwSize a7, mwSize a8, mwSize a9);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclGetLogicalSparseInferRowsCols_proxy(void **a0, mwSize a1, 
    const mwIndex *a2, mwSize a3, const mwIndex *a4, mwSize a5, 
    const mxLogical *a6, mwSize a7);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclDeserializeArray_proxy(void **a0, void **a1);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclcppGetArrayBuffer_proxy(void **a0, mwSize a1);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclcppFeval_proxy(HMCRINSTANCE a0, const char *a1, int a2, void **a3, 
    void *a4);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclcppArrayToString_proxy(mxArray *a0, char **a1);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
void mclcppFreeString_proxy(char *a0);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclmxArray2ArrayHandle_proxy(void **a0, mxArray *a1);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclArrayHandle2mxArray_proxy(mxArray **a0, void *a1);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclMXArrayGetIndexArrays_proxy(mxArray **a0, mxArray **a1, 
    mxArray *a2);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclMXArrayGet_proxy(mxArray **a0, mxArray *a1, mwSize a2, 
    const mwIndex *a3);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclMXArrayGetReal_proxy(mxArray **a0, mxArray *a1, mwSize a2, 
    const mwIndex *a3);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclMXArrayGetImag_proxy(mxArray **a0, mxArray *a1, mwSize a2, 
    const mwIndex *a3);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclMXArraySet_proxy(mxArray *a0, mxArray *a1, mwSize a2, 
    const mwIndex *a3);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclMXArraySetReal_proxy(mxArray *a0, mxArray *a1, mwSize a2, 
    const mwIndex *a3);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclMXArraySetImag_proxy(mxArray *a0, mxArray *a1, mwSize a2, 
    const mwIndex *a3);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclMXArraySetLogical_proxy(mxArray *a0, mxArray *a1, mwSize a2, 
    const mwIndex *a3);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
void mclMxRefDestroyArray_proxy(mxArray *a0);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
mxArray * mclMxRefSerialize_proxy(mxArray *a0);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
mxArray * mclMxRefDeserialize_proxy(const void *a0, size_t a1, size_t a2);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
size_t mclMxRefMvmId_proxy(mxArray *a0);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
size_t mclHashNBytes_proxy(size_t a0, size_t a1, const char *a2);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
mwIndex mclCalcSingleSubscript_proxy(const mxArray *a0, mwSize a1, 
    const mwIndex *a2);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
mxArray * mclCreateCharMatrixFromUTF16Strings_proxy(mwSize a0, 
    const mxChar **a1);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mcl2DCharArrayToUTF16Strings_proxy(const mxArray *a0, mxChar **a1, 
    mwSize *a2);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclWrite_proxy(int a0, const void *a1, size_t a2);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
void mclAddCanonicalPathMacro_proxy(const char *a0, const char *a1);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
bool mclFevalInternal_proxy(HMCRINSTANCE a0, const char *a1, int a2, 
    mxArray **a3, int a4, mxArray **a5);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclGetMaxPathLen_proxy();

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
bool mclmcrInitializeInternal2_proxy(int a0);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
bool mclmcrInitializeInternal_proxy();

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
void deleteWcsStackPointer_hPtr_proxy(pwcsStackPointer a0);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
void initializeWcsStackPointer_proxy(pwcsStackPointer *a0);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
void deleteWcsStackPointer_proxy(pwcsStackPointer a0);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
bool allocWcsStackPointer_proxy(pwcsStackPointer *a0, int a1);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mwMbstowcs_proxy(pwcsStackPointer a0, const char *a1);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
void utf16_to_lcp_n_fcn_proxy(char *a0, int32_t *a1, const CHAR16_T *a2, 
    int32_t a3);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int32_t utf16_strlen_fcn_proxy(const CHAR16_T *a0);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
CHAR16_T * utf16_strncpy_fcn_proxy(CHAR16_T *a0, const CHAR16_T *a1, 
    int32_t a2);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
CHAR16_T * utf16_strdup_fcn_proxy(const CHAR16_T *a0);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
bool mclSetGlobal_proxy(HMCRINSTANCE a0, const char *a1, mxArray *a2);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
bool mclIsStandaloneMode_proxy();

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
bool mclImpersonationFeval_proxy(HMCRINSTANCE a0, const char *a1, int a2, 
    mxArray **a3, int a4, mxArray **a5, void *a6);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
bool mclGetGlobal_proxy(HMCRINSTANCE a0, const char *a1, mxArray **a2);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
long int mclGetID_proxy(HMCRINSTANCE a0);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int mclMain_proxy(HMCRINSTANCE a0, int a1, const char **a2, 
    const char *a3, int a4);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
bool mclMlfVFevalInternal_proxy(HMCRINSTANCE a0, const char *a1, int a2, 
    int a3, int a4, va_list a5);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
bool mclGetMCRVersion_proxy(const char **a0);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
size_t mclGetActiveID_proxy();

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
char * mclGetTempFileName_proxy(char *a0);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
bool mclTerminateInstance_proxy(HMCRINSTANCE *a0);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
void stopImpersonationOnMCRThread_proxy(HMCRINSTANCE a0);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
bool mclMxIsA_proxy(HMCRINSTANCE a0, mxArray *a1, const char *a2);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
bool mclMxIsRef_proxy(mxArray *a0);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
bool mclMxRefIsA_proxy(mxArray *a0, const char *a1);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
const char * mclMxRefGetRefClassName_proxy(const mxArray *a0);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
mxArray * mclMxRefGetProperty_proxy(const mxArray *a0, mwIndex a1, 
    const char *a2);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
void mclMxRefSetProperty_proxy(mxArray *a0, mwIndex a1, const char *a2, 
    const mxArray *a3);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
mxArray * mclMxReleaseRef_proxy(mxArray *a0);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
MVMID_t mclMxRefLocalMvm_proxy(mxArray *a0);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
void mclMxDestroyArray_proxy(HMCRINSTANCE a0, mxArray *a1);

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
void mclNonDefaultAppDomainInUse_proxy();

/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
bool mclIsNonDefaultAppDomainInUse_proxy();

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int ref_count_obj_addref_proxy(class ref_count_obj *a0);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int ref_count_obj_release_proxy(class ref_count_obj *a0);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
size_t char_buffer_size_proxy(class char_buffer *a0);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
const char * char_buffer_get_buffer_proxy(class char_buffer *a0);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int char_buffer_set_buffer_proxy(class char_buffer *a0, const char *a1);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int char_buffer_compare_to_proxy(class char_buffer *a0, 
    class char_buffer *a1);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
mxClassID array_ref_classID_proxy(class array_ref *a0);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
class array_ref * array_ref_deep_copy_proxy(class array_ref *a0);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
void array_ref_detach_proxy(class array_ref *a0);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
class array_ref * array_ref_shared_copy_proxy(class array_ref *a0);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
class array_ref * array_ref_serialize_proxy(class array_ref *a0);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
size_t array_ref_element_size_proxy(class array_ref *a0);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
mwSize array_ref_number_of_elements_proxy(class array_ref *a0);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
mwSize array_ref_number_of_nonzeros_proxy(class array_ref *a0);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
mwSize array_ref_maximum_nonzeros_proxy(class array_ref *a0);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
mwSize array_ref_number_of_dimensions_proxy(class array_ref *a0);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
class array_ref * array_ref_get_dimensions_proxy(class array_ref *a0);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_ref_number_of_fields_proxy(class array_ref *a0);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
class char_buffer * array_ref_get_field_name_proxy(class array_ref *a0, 
    int a1);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
bool array_ref_is_empty_proxy(class array_ref *a0);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
bool array_ref_is_sparse_proxy(class array_ref *a0);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
bool array_ref_is_numeric_proxy(class array_ref *a0);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
bool array_ref_is_complex_proxy(class array_ref *a0);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
bool array_ref_is_matlab_string_proxy(class array_ref *a0);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
bool array_ref_is_missing_string_element_proxy(class array_ref *a0, 
    mwSize a1);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_ref_make_complex_proxy(class array_ref *a0);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
bool array_ref_equals_proxy(class array_ref *a0, class array_ref *a1);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_ref_compare_to_proxy(class array_ref *a0, class array_ref *a1);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_ref_hash_code_proxy(class array_ref *a0);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
class char_buffer * array_ref_to_string_proxy(class array_ref *a0);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
class array_ref * array_ref_row_index_proxy(class array_ref *a0);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
class array_ref * array_ref_column_index_proxy(class array_ref *a0);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
class array_ref * array_ref_get_int_proxy(class array_ref *a0, mwSize a1, 
    const mwIndex *a2);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
class array_ref * array_ref_get_const_char_proxy(class array_ref *a0, 
    const char *a1, mwSize a2, const mwIndex *a3);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
class array_ref * array_ref_getV_int_proxy(class array_ref *a0, 
    mwSize a1, va_list a2);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
class array_ref * array_ref_getV_const_char_proxy(class array_ref *a0, 
    const char *a1, mwSize a2, va_list a3);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_ref_set_proxy(class array_ref *a0, class array_ref *a1);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
class array_ref * array_ref_real_proxy(class array_ref *a0);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
class array_ref * array_ref_imag_proxy(class array_ref *a0);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_ref_get_numeric_mxDouble_proxy(class array_ref *a0, 
    mxDouble *a1, mwSize a2);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_ref_get_numeric_mxSingle_proxy(class array_ref *a0, 
    mxSingle *a1, mwSize a2);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_ref_get_numeric_mxInt8_proxy(class array_ref *a0, mxInt8 *a1, 
    mwSize a2);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_ref_get_numeric_mxUint8_proxy(class array_ref *a0, mxUint8 *a1, 
    mwSize a2);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_ref_get_numeric_mxInt16_proxy(class array_ref *a0, mxInt16 *a1, 
    mwSize a2);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_ref_get_numeric_mxUint16_proxy(class array_ref *a0, 
    mxUint16 *a1, mwSize a2);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_ref_get_numeric_mxInt32_proxy(class array_ref *a0, mxInt32 *a1, 
    mwSize a2);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_ref_get_numeric_mxUint32_proxy(class array_ref *a0, 
    mxUint32 *a1, mwSize a2);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_ref_get_numeric_mxInt64_proxy(class array_ref *a0, mxInt64 *a1, 
    mwSize a2);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_ref_get_numeric_mxUint64_proxy(class array_ref *a0, 
    mxUint64 *a1, mwSize a2);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_ref_get_char_proxy(class array_ref *a0, mxChar *a1, mwSize a2);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_ref_get_string_element_proxy(class array_ref *a0, mwSize a1, 
    const mxChar **a2, mwSize *a3);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_ref_get_matlab_string_proxy(class array_ref *a0, 
    const mxChar **a1, mwSize *a2, mwSize *a3);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_ref_get_logical_proxy(class array_ref *a0, mxLogical *a1, 
    mwSize a2);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_ref_set_numeric_mxDouble_proxy(class array_ref *a0, 
    const mxDouble *a1, mwSize a2);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_ref_set_numeric_mxSingle_proxy(class array_ref *a0, 
    const mxSingle *a1, mwSize a2);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_ref_set_numeric_mxInt8_proxy(class array_ref *a0, 
    const mxInt8 *a1, mwSize a2);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_ref_set_numeric_mxUint8_proxy(class array_ref *a0, 
    const mxUint8 *a1, mwSize a2);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_ref_set_numeric_mxInt16_proxy(class array_ref *a0, 
    const mxInt16 *a1, mwSize a2);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_ref_set_numeric_mxUint16_proxy(class array_ref *a0, 
    const mxUint16 *a1, mwSize a2);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_ref_set_numeric_mxInt32_proxy(class array_ref *a0, 
    const mxInt32 *a1, mwSize a2);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_ref_set_numeric_mxUint32_proxy(class array_ref *a0, 
    const mxUint32 *a1, mwSize a2);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_ref_set_numeric_mxInt64_proxy(class array_ref *a0, 
    const mxInt64 *a1, mwSize a2);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_ref_set_numeric_mxUint64_proxy(class array_ref *a0, 
    const mxUint64 *a1, mwSize a2);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_ref_set_char_proxy(class array_ref *a0, const mxChar *a1, 
    mwSize a2);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_ref_set_string_element_proxy(class array_ref *a0, mwSize a1, 
    const mxChar *a2);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_ref_set_matlab_string_proxy(class array_ref *a0, 
    const mxChar **a1, mwSize a2);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_ref_set_logical_proxy(class array_ref *a0, const mxLogical *a1, 
    mwSize a2);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
mwSize array_buffer_size_proxy(class array_buffer *a0);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
class array_ref * array_buffer_get_proxy(class array_buffer *a0, 
    mwIndex a1);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_buffer_set_proxy(class array_buffer *a0, mwIndex a1, 
    class array_ref *a2);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_buffer_add_proxy(class array_buffer *a0, class array_ref *a1);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_buffer_remove_proxy(class array_buffer *a0, mwIndex a1);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_buffer_clear_proxy(class array_buffer *a0);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
class array_ref * array_buffer_to_cell_proxy(class array_buffer *a0, 
    mwIndex a1, mwSize a2);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
const char * error_info_get_message_proxy(class error_info *a0);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
size_t error_info_get_stack_trace_proxy(class error_info *a0, char ***a1);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
mxClassID array_handle_classID_proxy(array_handle a0);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
array_handle array_handle_deep_copy_proxy(array_handle a0);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
void array_handle_detach_proxy(array_handle a0);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
array_handle array_handle_shared_copy_proxy(array_handle a0);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
array_handle array_handle_serialize_proxy(array_handle a0);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
size_t array_handle_element_size_proxy(array_handle a0);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
mwSize array_handle_number_of_elements_proxy(array_handle a0);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
mwSize array_handle_number_of_nonzeros_proxy(array_handle a0);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
mwSize array_handle_maximum_nonzeros_proxy(array_handle a0);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
mwSize array_handle_number_of_dimensions_proxy(array_handle a0);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
array_handle array_handle_get_dimensions_proxy(array_handle a0);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_handle_number_of_fields_proxy(array_handle a0);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
class char_buffer * array_handle_get_field_name_proxy(array_handle a0, 
    int a1);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
bool array_handle_is_empty_proxy(array_handle a0);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
bool array_handle_is_sparse_proxy(array_handle a0);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
bool array_handle_is_numeric_proxy(array_handle a0);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
bool array_handle_is_complex_proxy(array_handle a0);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_handle_make_complex_proxy(array_handle a0);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
bool array_handle_equals_proxy(array_handle a0, array_handle a1);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_handle_compare_to_proxy(array_handle a0, array_handle a1);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_handle_hash_code_proxy(array_handle a0);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
class char_buffer * array_handle_to_string_proxy(array_handle a0);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
array_handle array_handle_row_index_proxy(array_handle a0);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
array_handle array_handle_column_index_proxy(array_handle a0);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
array_handle array_handle_get_int_proxy(array_handle a0, mwSize a1, 
    const mwIndex *a2);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
array_handle array_handle_get_const_char_proxy(array_handle a0, 
    const char *a1, mwSize a2, const mwIndex *a3);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
array_handle array_handle_getV_int_proxy(array_handle a0, mwSize a1, 
    va_list a2);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
array_handle array_handle_getV_const_char_proxy(array_handle a0, 
    const char *a1, mwSize a2, va_list a3);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_handle_set_proxy(array_handle a0, array_handle a1);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
array_handle array_handle_real_proxy(array_handle a0);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
array_handle array_handle_imag_proxy(array_handle a0);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_handle_get_numeric_mxDouble_proxy(array_handle a0, 
    mxDouble *a1, mwSize a2);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_handle_get_numeric_mxSingle_proxy(array_handle a0, 
    mxSingle *a1, mwSize a2);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_handle_get_numeric_mxInt8_proxy(array_handle a0, mxInt8 *a1, 
    mwSize a2);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_handle_get_numeric_mxUint8_proxy(array_handle a0, mxUint8 *a1, 
    mwSize a2);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_handle_get_numeric_mxInt16_proxy(array_handle a0, mxInt16 *a1, 
    mwSize a2);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_handle_get_numeric_mxUint16_proxy(array_handle a0, 
    mxUint16 *a1, mwSize a2);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_handle_get_numeric_mxInt32_proxy(array_handle a0, mxInt32 *a1, 
    mwSize a2);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_handle_get_numeric_mxUint32_proxy(array_handle a0, 
    mxUint32 *a1, mwSize a2);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_handle_get_numeric_mxInt64_proxy(array_handle a0, mxInt64 *a1, 
    mwSize a2);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_handle_get_numeric_mxUint64_proxy(array_handle a0, 
    mxUint64 *a1, mwSize a2);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_handle_get_char_proxy(array_handle a0, mxChar *a1, mwSize a2);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_handle_get_logical_proxy(array_handle a0, mxLogical *a1, 
    mwSize a2);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_handle_set_numeric_mxDouble_proxy(array_handle a0, 
    const mxDouble *a1, mwSize a2);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_handle_set_numeric_mxSingle_proxy(array_handle a0, 
    const mxSingle *a1, mwSize a2);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_handle_set_numeric_mxInt8_proxy(array_handle a0, 
    const mxInt8 *a1, mwSize a2);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_handle_set_numeric_mxUint8_proxy(array_handle a0, 
    const mxUint8 *a1, mwSize a2);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_handle_set_numeric_mxInt16_proxy(array_handle a0, 
    const mxInt16 *a1, mwSize a2);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_handle_set_numeric_mxUint16_proxy(array_handle a0, 
    const mxUint16 *a1, mwSize a2);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_handle_set_numeric_mxInt32_proxy(array_handle a0, 
    const mxInt32 *a1, mwSize a2);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_handle_set_numeric_mxUint32_proxy(array_handle a0, 
    const mxUint32 *a1, mwSize a2);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_handle_set_numeric_mxInt64_proxy(array_handle a0, 
    const mxInt64 *a1, mwSize a2);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_handle_set_numeric_mxUint64_proxy(array_handle a0, 
    const mxUint64 *a1, mwSize a2);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_handle_set_char_proxy(array_handle a0, const mxChar *a1, 
    mwSize a2);
#endif /* __cplusplus */

#ifdef __cplusplus /* Only available in C++ */
/**This function is for INTERNAL USE ONLY.**/
EXTERN_C
int array_handle_set_logical_proxy(array_handle a0, const mxLogical *a1, 
    mwSize a2);
#endif /* __cplusplus */


#if !defined(MW_BUILD_ARCHIVES)
#ifdef __cplusplus
extern "C"
{
#endif


#undef mclMlfVFeval
#define mclMlfVFeval mclMlfVFeval_proxy


#ifdef __cplusplus
}
#endif
#endif


#ifdef __cplusplus
extern "C"
{
#endif


#if !defined(MW_BUILD_ARCHIVES)
#undef mclFeval
#define mclFeval mclFeval_proxy
#endif


EXTERN_C
bool MW_CALL_CONV mclFeval_proxy(HMCRINSTANCE a0, const char *a1, int a2,
                                 mxArray **a3, int a4, mxArray **a5);



#ifdef __cplusplus
}
#endif


#ifdef __cplusplus
extern "C"
{
#endif


#if !defined(MW_BUILD_ARCHIVES)
#undef mclMlfFeval
#define mclMlfFeval mclMlfFeval_proxy
#endif


EXTERN_C
bool MW_CALL_CONV mclMlfFeval_proxy(HMCRINSTANCE a0, const char *a1,
		  	            int a2, int a3, int a4, ...);


#ifdef __cplusplus
}
#endif

#ifndef MW_CALL_CONV
#  ifdef _WIN32 
#      define MW_CALL_CONV __cdecl
#  else
#      define MW_CALL_CONV 
#  endif
#endif

#if defined(MX_COMPAT_32)
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetNumberOfDimensions_700
#define mxGetNumberOfDimensions_700 mxGetNumberOfDimensions_700_proxy
#endif
#endif   /* defined(MX_COMPAT_32) */



#if defined(MX_COMPAT_32)
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetDimensions_700
#define mxGetDimensions_700 mxGetDimensions_700_proxy
#endif
#endif   /* defined(MX_COMPAT_32) */



#if defined(MX_COMPAT_32)
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetIr_700
#define mxGetIr_700 mxGetIr_700_proxy
#endif
#endif   /* defined(MX_COMPAT_32) */



#if defined(MX_COMPAT_32)
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetJc_700
#define mxGetJc_700 mxGetJc_700_proxy
#endif
#endif   /* defined(MX_COMPAT_32) */



#if defined(MX_COMPAT_32)
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetNzmax_700
#define mxGetNzmax_700 mxGetNzmax_700_proxy
#endif
#endif   /* defined(MX_COMPAT_32) */



#if defined(MX_COMPAT_32)
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetNzmax_700
#define mxSetNzmax_700 mxSetNzmax_700_proxy
#endif
#endif   /* defined(MX_COMPAT_32) */



#if defined(MX_COMPAT_32)
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetFieldByNumber_700
#define mxGetFieldByNumber_700 mxGetFieldByNumber_700_proxy
#endif
#endif   /* defined(MX_COMPAT_32) */



#if defined(MX_COMPAT_32)
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetCell_700
#define mxGetCell_700 mxGetCell_700_proxy
#endif
#endif   /* defined(MX_COMPAT_32) */



#if defined(MX_COMPAT_32)
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetM_700
#define mxSetM_700 mxSetM_700_proxy
#endif
#endif   /* defined(MX_COMPAT_32) */



#if defined(MX_COMPAT_32)
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetIr_700
#define mxSetIr_700 mxSetIr_700_proxy
#endif
#endif   /* defined(MX_COMPAT_32) */



#if defined(MX_COMPAT_32)
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetJc_700
#define mxSetJc_700 mxSetJc_700_proxy
#endif
#endif   /* defined(MX_COMPAT_32) */



#if defined(MX_COMPAT_32)
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCalcSingleSubscript_700
#define mxCalcSingleSubscript_700 mxCalcSingleSubscript_700_proxy
#endif
#endif   /* defined(MX_COMPAT_32) */



#if defined(MX_COMPAT_32)
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetCell_700
#define mxSetCell_700 mxSetCell_700_proxy
#endif
#endif   /* defined(MX_COMPAT_32) */



#if defined(MX_COMPAT_32)
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetFieldByNumber_700
#define mxSetFieldByNumber_700 mxSetFieldByNumber_700_proxy
#endif
#endif   /* defined(MX_COMPAT_32) */



#if defined(MX_COMPAT_32)
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetField_700
#define mxGetField_700 mxGetField_700_proxy
#endif
#endif   /* defined(MX_COMPAT_32) */



#if defined(MX_COMPAT_32)
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetField_700
#define mxSetField_700 mxSetField_700_proxy
#endif
#endif   /* defined(MX_COMPAT_32) */



#if defined(MX_COMPAT_32)
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCreateNumericMatrix_700
#define mxCreateNumericMatrix_700 mxCreateNumericMatrix_700_proxy
#endif
#endif   /* defined(MX_COMPAT_32) */



#if defined(MX_COMPAT_32)
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetN_700
#define mxSetN_700 mxSetN_700_proxy
#endif
#endif   /* defined(MX_COMPAT_32) */



#if defined(MX_COMPAT_32)
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetDimensions_700
#define mxSetDimensions_700 mxSetDimensions_700_proxy
#endif
#endif   /* defined(MX_COMPAT_32) */



#if defined(MX_COMPAT_32)
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCreateNumericArray_700
#define mxCreateNumericArray_700 mxCreateNumericArray_700_proxy
#endif
#endif   /* defined(MX_COMPAT_32) */



#if defined(MX_COMPAT_32)
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCreateCharArray_700
#define mxCreateCharArray_700 mxCreateCharArray_700_proxy
#endif
#endif   /* defined(MX_COMPAT_32) */



#if defined(MX_COMPAT_32)
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCreateDoubleMatrix_700
#define mxCreateDoubleMatrix_700 mxCreateDoubleMatrix_700_proxy
#endif
#endif   /* defined(MX_COMPAT_32) */



#if defined(MX_COMPAT_32)
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCreateLogicalArray_700
#define mxCreateLogicalArray_700 mxCreateLogicalArray_700_proxy
#endif
#endif   /* defined(MX_COMPAT_32) */



#if defined(MX_COMPAT_32)
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCreateLogicalMatrix_700
#define mxCreateLogicalMatrix_700 mxCreateLogicalMatrix_700_proxy
#endif
#endif   /* defined(MX_COMPAT_32) */



#if defined(MX_COMPAT_32)
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCreateSparse_700
#define mxCreateSparse_700 mxCreateSparse_700_proxy
#endif
#endif   /* defined(MX_COMPAT_32) */



#if defined(MX_COMPAT_32)
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCreateSparseLogicalMatrix_700
#define mxCreateSparseLogicalMatrix_700 mxCreateSparseLogicalMatrix_700_proxy
#endif
#endif   /* defined(MX_COMPAT_32) */



#if defined(MX_COMPAT_32)
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetNChars_700
#define mxGetNChars_700 mxGetNChars_700_proxy
#endif
#endif   /* defined(MX_COMPAT_32) */



#if defined(MX_COMPAT_32)
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetString_700
#define mxGetString_700 mxGetString_700_proxy
#endif
#endif   /* defined(MX_COMPAT_32) */



#if defined(MX_COMPAT_32)
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCreateStringFromNChars_700
#define mxCreateStringFromNChars_700 mxCreateStringFromNChars_700_proxy
#endif
#endif   /* defined(MX_COMPAT_32) */



#if defined(MX_COMPAT_32)
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCreateCharMatrixFromStrings_700
#define mxCreateCharMatrixFromStrings_700 mxCreateCharMatrixFromStrings_700_proxy
#endif
#endif   /* defined(MX_COMPAT_32) */



#if defined(MX_COMPAT_32)
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCreateCellMatrix_700
#define mxCreateCellMatrix_700 mxCreateCellMatrix_700_proxy
#endif
#endif   /* defined(MX_COMPAT_32) */



#if defined(MX_COMPAT_32)
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCreateCellArray_700
#define mxCreateCellArray_700 mxCreateCellArray_700_proxy
#endif
#endif   /* defined(MX_COMPAT_32) */



#if defined(MX_COMPAT_32)
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCreateStructMatrix_700
#define mxCreateStructMatrix_700 mxCreateStructMatrix_700_proxy
#endif
#endif   /* defined(MX_COMPAT_32) */



#if defined(MX_COMPAT_32)
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCreateStructArray_700
#define mxCreateStructArray_700 mxCreateStructArray_700_proxy
#endif
#endif   /* defined(MX_COMPAT_32) */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxMalloc
#define mxMalloc mxMalloc_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCalloc
#define mxCalloc mxCalloc_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxFree
#define mxFree mxFree_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxRealloc
#define mxRealloc mxRealloc_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetNumberOfDimensions_730
#define mxGetNumberOfDimensions_730 mxGetNumberOfDimensions_730_proxy
#endif
#endif   /* !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800 */



#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetDimensions_730
#define mxGetDimensions_730 mxGetDimensions_730_proxy
#endif
#endif   /* !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetM
#define mxGetM mxGetM_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetIr_730
#define mxGetIr_730 mxGetIr_730_proxy
#endif
#endif   /* !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800 */



#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetJc_730
#define mxGetJc_730 mxGetJc_730_proxy
#endif
#endif   /* !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800 */



#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetNzmax_730
#define mxGetNzmax_730 mxGetNzmax_730_proxy
#endif
#endif   /* !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800 */



#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetNzmax_730
#define mxSetNzmax_730 mxSetNzmax_730_proxy
#endif
#endif   /* !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetFieldNameByNumber
#define mxGetFieldNameByNumber mxGetFieldNameByNumber_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetFieldByNumber_730
#define mxGetFieldByNumber_730 mxGetFieldByNumber_730_proxy
#endif
#endif   /* !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800 */



#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetCell_730
#define mxGetCell_730 mxGetCell_730_proxy
#endif
#endif   /* !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetClassID
#define mxGetClassID mxGetClassID_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetData
#define mxGetData mxGetData_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetData
#define mxSetData mxSetData_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsNumeric
#define mxIsNumeric mxIsNumeric_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsCell
#define mxIsCell mxIsCell_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsLogical
#define mxIsLogical mxIsLogical_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsScalar
#define mxIsScalar mxIsScalar_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsChar
#define mxIsChar mxIsChar_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsStruct
#define mxIsStruct mxIsStruct_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsOpaque
#define mxIsOpaque mxIsOpaque_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsFunctionHandle
#define mxIsFunctionHandle mxIsFunctionHandle_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsObject
#define mxIsObject mxIsObject_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetImagData
#define mxGetImagData mxGetImagData_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetImagData
#define mxSetImagData mxSetImagData_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsComplex
#define mxIsComplex mxIsComplex_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsSparse
#define mxIsSparse mxIsSparse_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsDouble
#define mxIsDouble mxIsDouble_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsSingle
#define mxIsSingle mxIsSingle_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsInt8
#define mxIsInt8 mxIsInt8_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsUint8
#define mxIsUint8 mxIsUint8_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsInt16
#define mxIsInt16 mxIsInt16_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsUint16
#define mxIsUint16 mxIsUint16_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsInt32
#define mxIsInt32 mxIsInt32_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsUint32
#define mxIsUint32 mxIsUint32_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsInt64
#define mxIsInt64 mxIsInt64_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsUint64
#define mxIsUint64 mxIsUint64_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetNumberOfElements
#define mxGetNumberOfElements mxGetNumberOfElements_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetPi
#define mxGetPi mxGetPi_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetPi
#define mxSetPi mxSetPi_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetChars
#define mxGetChars mxGetChars_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetUserBits
#define mxGetUserBits mxGetUserBits_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetUserBits
#define mxSetUserBits mxSetUserBits_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetScalar
#define mxGetScalar mxGetScalar_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsFromGlobalWS
#define mxIsFromGlobalWS mxIsFromGlobalWS_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetFromGlobalWS
#define mxSetFromGlobalWS mxSetFromGlobalWS_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetM_730
#define mxSetM_730 mxSetM_730_proxy
#endif
#endif   /* !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetN
#define mxGetN mxGetN_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsEmpty
#define mxIsEmpty mxIsEmpty_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetFieldNumber
#define mxGetFieldNumber mxGetFieldNumber_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetIr_730
#define mxSetIr_730 mxSetIr_730_proxy
#endif
#endif   /* !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800 */



#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetJc_730
#define mxSetJc_730 mxSetJc_730_proxy
#endif
#endif   /* !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetPr
#define mxGetPr mxGetPr_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetPr
#define mxSetPr mxSetPr_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetElementSize
#define mxGetElementSize mxGetElementSize_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCalcSingleSubscript_730
#define mxCalcSingleSubscript_730 mxCalcSingleSubscript_730_proxy
#endif
#endif   /* !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetNumberOfFields
#define mxGetNumberOfFields mxGetNumberOfFields_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetCell_730
#define mxSetCell_730 mxSetCell_730_proxy
#endif
#endif   /* !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800 */



#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetFieldByNumber_730
#define mxSetFieldByNumber_730 mxSetFieldByNumber_730_proxy
#endif
#endif   /* !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800 */



#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetField_730
#define mxGetField_730 mxGetField_730_proxy
#endif
#endif   /* !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800 */



#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetField_730
#define mxSetField_730 mxSetField_730_proxy
#endif
#endif   /* !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsClass
#define mxIsClass mxIsClass_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCreateNumericMatrix_730
#define mxCreateNumericMatrix_730 mxCreateNumericMatrix_730_proxy
#endif
#endif   /* !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCreateUninitNumericMatrix
#define mxCreateUninitNumericMatrix mxCreateUninitNumericMatrix_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCreateUninitNumericArray
#define mxCreateUninitNumericArray mxCreateUninitNumericArray_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetN_730
#define mxSetN_730 mxSetN_730_proxy
#endif
#endif   /* !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800 */



#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetDimensions_730
#define mxSetDimensions_730 mxSetDimensions_730_proxy
#endif
#endif   /* !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxDestroyArray
#define mxDestroyArray mxDestroyArray_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCreateNumericArray_730
#define mxCreateNumericArray_730 mxCreateNumericArray_730_proxy
#endif
#endif   /* !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800 */



#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCreateCharArray_730
#define mxCreateCharArray_730 mxCreateCharArray_730_proxy
#endif
#endif   /* !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800 */



#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCreateDoubleMatrix_730
#define mxCreateDoubleMatrix_730 mxCreateDoubleMatrix_730_proxy
#endif
#endif   /* !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetLogicals
#define mxGetLogicals mxGetLogicals_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCreateLogicalArray_730
#define mxCreateLogicalArray_730 mxCreateLogicalArray_730_proxy
#endif
#endif   /* !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800 */



#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCreateLogicalMatrix_730
#define mxCreateLogicalMatrix_730 mxCreateLogicalMatrix_730_proxy
#endif
#endif   /* !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCreateLogicalScalar
#define mxCreateLogicalScalar mxCreateLogicalScalar_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsLogicalScalar
#define mxIsLogicalScalar mxIsLogicalScalar_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsLogicalScalarTrue
#define mxIsLogicalScalarTrue mxIsLogicalScalarTrue_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCreateDoubleScalar
#define mxCreateDoubleScalar mxCreateDoubleScalar_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCreateSparse_730
#define mxCreateSparse_730 mxCreateSparse_730_proxy
#endif
#endif   /* !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800 */



#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCreateSparseLogicalMatrix_730
#define mxCreateSparseLogicalMatrix_730 mxCreateSparseLogicalMatrix_730_proxy
#endif
#endif   /* !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800 */



#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetNChars_730
#define mxGetNChars_730 mxGetNChars_730_proxy
#endif
#endif   /* !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800 */



#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetString_730
#define mxGetString_730 mxGetString_730_proxy
#endif
#endif   /* !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxArrayToString
#define mxArrayToString mxArrayToString_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxArrayToUTF8String
#define mxArrayToUTF8String mxArrayToUTF8String_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCreateStringFromNChars_730
#define mxCreateStringFromNChars_730 mxCreateStringFromNChars_730_proxy
#endif
#endif   /* !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCreateString
#define mxCreateString mxCreateString_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCreateCharMatrixFromStrings_730
#define mxCreateCharMatrixFromStrings_730 mxCreateCharMatrixFromStrings_730_proxy
#endif
#endif   /* !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800 */



#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCreateCellMatrix_730
#define mxCreateCellMatrix_730 mxCreateCellMatrix_730_proxy
#endif
#endif   /* !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800 */



#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCreateCellArray_730
#define mxCreateCellArray_730 mxCreateCellArray_730_proxy
#endif
#endif   /* !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800 */



#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCreateStructMatrix_730
#define mxCreateStructMatrix_730 mxCreateStructMatrix_730_proxy
#endif
#endif   /* !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800 */



#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCreateStructArray_730
#define mxCreateStructArray_730 mxCreateStructArray_730_proxy
#endif
#endif   /* !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxDuplicateArray
#define mxDuplicateArray mxDuplicateArray_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetClassName
#define mxSetClassName mxSetClassName_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxAddField
#define mxAddField mxAddField_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxRemoveField
#define mxRemoveField mxRemoveField_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetEps
#define mxGetEps mxGetEps_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetInf
#define mxGetInf mxGetInf_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetNaN
#define mxGetNaN mxGetNaN_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsFinite
#define mxIsFinite mxIsFinite_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsInf
#define mxIsInf mxIsInf_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsNaN
#define mxIsNaN mxIsNaN_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCreateSharedDataCopy
#define mxCreateSharedDataCopy mxCreateSharedDataCopy_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCreateUninitDoubleMatrix
#define mxCreateUninitDoubleMatrix mxCreateUninitDoubleMatrix_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxFastZeros
#define mxFastZeros mxFastZeros_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxUnreference
#define mxUnreference mxUnreference_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION < 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxUnshareArray
#define mxUnshareArray mxUnshareArray_proxy
#endif
#endif   /* TARGET_API_VERSION < 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxMalloc_800
#define mxMalloc_800 mxMalloc_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCalloc_800
#define mxCalloc_800 mxCalloc_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxFree_800
#define mxFree_800 mxFree_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxRealloc_800
#define mxRealloc_800 mxRealloc_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetNumberOfDimensions_800
#define mxGetNumberOfDimensions_800 mxGetNumberOfDimensions_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetDimensions_800
#define mxGetDimensions_800 mxGetDimensions_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetM_800
#define mxGetM_800 mxGetM_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetIr_800
#define mxGetIr_800 mxGetIr_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetJc_800
#define mxGetJc_800 mxGetJc_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetNzmax_800
#define mxGetNzmax_800 mxGetNzmax_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetNzmax_800
#define mxSetNzmax_800 mxSetNzmax_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetFieldNameByNumber_800
#define mxGetFieldNameByNumber_800 mxGetFieldNameByNumber_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetFieldByNumber_800
#define mxGetFieldByNumber_800 mxGetFieldByNumber_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetCell_800
#define mxGetCell_800 mxGetCell_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetClassID_800
#define mxGetClassID_800 mxGetClassID_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetData_800
#define mxGetData_800 mxGetData_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetData_800
#define mxSetData_800 mxSetData_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsNumeric_800
#define mxIsNumeric_800 mxIsNumeric_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsCell_800
#define mxIsCell_800 mxIsCell_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsLogical_800
#define mxIsLogical_800 mxIsLogical_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsScalar_800
#define mxIsScalar_800 mxIsScalar_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsChar_800
#define mxIsChar_800 mxIsChar_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsStruct_800
#define mxIsStruct_800 mxIsStruct_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsOpaque_800
#define mxIsOpaque_800 mxIsOpaque_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsFunctionHandle_800
#define mxIsFunctionHandle_800 mxIsFunctionHandle_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsObject_800
#define mxIsObject_800 mxIsObject_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsComplex_800
#define mxIsComplex_800 mxIsComplex_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsSparse_800
#define mxIsSparse_800 mxIsSparse_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsDouble_800
#define mxIsDouble_800 mxIsDouble_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsSingle_800
#define mxIsSingle_800 mxIsSingle_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsInt8_800
#define mxIsInt8_800 mxIsInt8_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsUint8_800
#define mxIsUint8_800 mxIsUint8_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsInt16_800
#define mxIsInt16_800 mxIsInt16_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsUint16_800
#define mxIsUint16_800 mxIsUint16_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsInt32_800
#define mxIsInt32_800 mxIsInt32_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsUint32_800
#define mxIsUint32_800 mxIsUint32_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsInt64_800
#define mxIsInt64_800 mxIsInt64_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsUint64_800
#define mxIsUint64_800 mxIsUint64_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetNumberOfElements_800
#define mxGetNumberOfElements_800 mxGetNumberOfElements_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetChars_800
#define mxGetChars_800 mxGetChars_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetUserBits_800
#define mxGetUserBits_800 mxGetUserBits_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetUserBits_800
#define mxSetUserBits_800 mxSetUserBits_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetScalar_800
#define mxGetScalar_800 mxGetScalar_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsFromGlobalWS_800
#define mxIsFromGlobalWS_800 mxIsFromGlobalWS_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetFromGlobalWS_800
#define mxSetFromGlobalWS_800 mxSetFromGlobalWS_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetM_800
#define mxSetM_800 mxSetM_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetN_800
#define mxGetN_800 mxGetN_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsEmpty_800
#define mxIsEmpty_800 mxIsEmpty_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetFieldNumber_800
#define mxGetFieldNumber_800 mxGetFieldNumber_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetIr_800
#define mxSetIr_800 mxSetIr_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetJc_800
#define mxSetJc_800 mxSetJc_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetPr_800
#define mxGetPr_800 mxGetPr_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetPr_800
#define mxSetPr_800 mxSetPr_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetElementSize_800
#define mxGetElementSize_800 mxGetElementSize_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCalcSingleSubscript_800
#define mxCalcSingleSubscript_800 mxCalcSingleSubscript_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetNumberOfFields_800
#define mxGetNumberOfFields_800 mxGetNumberOfFields_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetCell_800
#define mxSetCell_800 mxSetCell_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetFieldByNumber_800
#define mxSetFieldByNumber_800 mxSetFieldByNumber_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetField_800
#define mxGetField_800 mxGetField_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetField_800
#define mxSetField_800 mxSetField_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsClass_800
#define mxIsClass_800 mxIsClass_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCreateNumericMatrix_800
#define mxCreateNumericMatrix_800 mxCreateNumericMatrix_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCreateUninitNumericMatrix_800
#define mxCreateUninitNumericMatrix_800 mxCreateUninitNumericMatrix_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCreateUninitNumericArray_800
#define mxCreateUninitNumericArray_800 mxCreateUninitNumericArray_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetN_800
#define mxSetN_800 mxSetN_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetDimensions_800
#define mxSetDimensions_800 mxSetDimensions_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxDestroyArray_800
#define mxDestroyArray_800 mxDestroyArray_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCreateNumericArray_800
#define mxCreateNumericArray_800 mxCreateNumericArray_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCreateCharArray_800
#define mxCreateCharArray_800 mxCreateCharArray_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCreateDoubleMatrix_800
#define mxCreateDoubleMatrix_800 mxCreateDoubleMatrix_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetLogicals_800
#define mxGetLogicals_800 mxGetLogicals_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCreateLogicalArray_800
#define mxCreateLogicalArray_800 mxCreateLogicalArray_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCreateLogicalMatrix_800
#define mxCreateLogicalMatrix_800 mxCreateLogicalMatrix_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCreateLogicalScalar_800
#define mxCreateLogicalScalar_800 mxCreateLogicalScalar_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsLogicalScalar_800
#define mxIsLogicalScalar_800 mxIsLogicalScalar_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsLogicalScalarTrue_800
#define mxIsLogicalScalarTrue_800 mxIsLogicalScalarTrue_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCreateDoubleScalar_800
#define mxCreateDoubleScalar_800 mxCreateDoubleScalar_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCreateSparse_800
#define mxCreateSparse_800 mxCreateSparse_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCreateSparseLogicalMatrix_800
#define mxCreateSparseLogicalMatrix_800 mxCreateSparseLogicalMatrix_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetNChars_800
#define mxGetNChars_800 mxGetNChars_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetString_800
#define mxGetString_800 mxGetString_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxArrayToString_800
#define mxArrayToString_800 mxArrayToString_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxArrayToUTF8String_800
#define mxArrayToUTF8String_800 mxArrayToUTF8String_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCreateStringFromNChars_800
#define mxCreateStringFromNChars_800 mxCreateStringFromNChars_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCreateString_800
#define mxCreateString_800 mxCreateString_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCreateCharMatrixFromStrings_800
#define mxCreateCharMatrixFromStrings_800 mxCreateCharMatrixFromStrings_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCreateCellMatrix_800
#define mxCreateCellMatrix_800 mxCreateCellMatrix_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCreateCellArray_800
#define mxCreateCellArray_800 mxCreateCellArray_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCreateStructMatrix_800
#define mxCreateStructMatrix_800 mxCreateStructMatrix_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxCreateStructArray_800
#define mxCreateStructArray_800 mxCreateStructArray_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxDuplicateArray_800
#define mxDuplicateArray_800 mxDuplicateArray_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetClassName_800
#define mxSetClassName_800 mxSetClassName_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxAddField_800
#define mxAddField_800 mxAddField_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxRemoveField_800
#define mxRemoveField_800 mxRemoveField_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetEps_800
#define mxGetEps_800 mxGetEps_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetInf_800
#define mxGetInf_800 mxGetInf_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetNaN_800
#define mxGetNaN_800 mxGetNaN_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsFinite_800
#define mxIsFinite_800 mxIsFinite_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsInf_800
#define mxIsInf_800 mxIsInf_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxIsNaN_800
#define mxIsNaN_800 mxIsNaN_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetDoubles_800
#define mxGetDoubles_800 mxGetDoubles_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetDoubles_800
#define mxSetDoubles_800 mxSetDoubles_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetComplexDoubles_800
#define mxGetComplexDoubles_800 mxGetComplexDoubles_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetComplexDoubles_800
#define mxSetComplexDoubles_800 mxSetComplexDoubles_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetSingles_800
#define mxGetSingles_800 mxGetSingles_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetSingles_800
#define mxSetSingles_800 mxSetSingles_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetComplexSingles_800
#define mxGetComplexSingles_800 mxGetComplexSingles_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetComplexSingles_800
#define mxSetComplexSingles_800 mxSetComplexSingles_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetInt8s_800
#define mxGetInt8s_800 mxGetInt8s_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetInt8s_800
#define mxSetInt8s_800 mxSetInt8s_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetComplexInt8s_800
#define mxGetComplexInt8s_800 mxGetComplexInt8s_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetComplexInt8s_800
#define mxSetComplexInt8s_800 mxSetComplexInt8s_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetUint8s_800
#define mxGetUint8s_800 mxGetUint8s_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetUint8s_800
#define mxSetUint8s_800 mxSetUint8s_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetComplexUint8s_800
#define mxGetComplexUint8s_800 mxGetComplexUint8s_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetComplexUint8s_800
#define mxSetComplexUint8s_800 mxSetComplexUint8s_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetInt16s_800
#define mxGetInt16s_800 mxGetInt16s_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetInt16s_800
#define mxSetInt16s_800 mxSetInt16s_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetComplexInt16s_800
#define mxGetComplexInt16s_800 mxGetComplexInt16s_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetComplexInt16s_800
#define mxSetComplexInt16s_800 mxSetComplexInt16s_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetUint16s_800
#define mxGetUint16s_800 mxGetUint16s_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetUint16s_800
#define mxSetUint16s_800 mxSetUint16s_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetComplexUint16s_800
#define mxGetComplexUint16s_800 mxGetComplexUint16s_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetComplexUint16s_800
#define mxSetComplexUint16s_800 mxSetComplexUint16s_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetInt32s_800
#define mxGetInt32s_800 mxGetInt32s_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetInt32s_800
#define mxSetInt32s_800 mxSetInt32s_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetComplexInt32s_800
#define mxGetComplexInt32s_800 mxGetComplexInt32s_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetComplexInt32s_800
#define mxSetComplexInt32s_800 mxSetComplexInt32s_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetUint32s_800
#define mxGetUint32s_800 mxGetUint32s_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetUint32s_800
#define mxSetUint32s_800 mxSetUint32s_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetComplexUint32s_800
#define mxGetComplexUint32s_800 mxGetComplexUint32s_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetComplexUint32s_800
#define mxSetComplexUint32s_800 mxSetComplexUint32s_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetInt64s_800
#define mxGetInt64s_800 mxGetInt64s_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetInt64s_800
#define mxSetInt64s_800 mxSetInt64s_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetComplexInt64s_800
#define mxGetComplexInt64s_800 mxGetComplexInt64s_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetComplexInt64s_800
#define mxSetComplexInt64s_800 mxSetComplexInt64s_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetUint64s_800
#define mxGetUint64s_800 mxGetUint64s_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetUint64s_800
#define mxSetUint64s_800 mxSetUint64s_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxGetComplexUint64s_800
#define mxGetComplexUint64s_800 mxGetComplexUint64s_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxSetComplexUint64s_800
#define mxSetComplexUint64s_800 mxSetComplexUint64s_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxMakeArrayReal_800
#define mxMakeArrayReal_800 mxMakeArrayReal_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */



#if TARGET_API_VERSION >= 800
/* Map original name to unique proxy layer name. */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#undef mxMakeArrayComplex_800
#define mxMakeArrayComplex_800 mxMakeArrayComplex_800_proxy
#endif
#endif   /* TARGET_API_VERSION >= 800 */




#define LIBMWMATRIX_API_EXTERN_C EXTERN_C

/* Proxies for functions in matrix.h */

#if defined(MX_COMPAT_32)
EXTERN_C
mwSize mxGetNumberOfDimensions_700_proxy(const mxArray *a0);
#endif

#if defined(MX_COMPAT_32)
EXTERN_C
const mwSize * mxGetDimensions_700_proxy(const mxArray *a0);
#endif

#if defined(MX_COMPAT_32)
EXTERN_C
mwIndex * mxGetIr_700_proxy(const mxArray *a0);
#endif

#if defined(MX_COMPAT_32)
EXTERN_C
mwIndex * mxGetJc_700_proxy(const mxArray *a0);
#endif

#if defined(MX_COMPAT_32)
EXTERN_C
mwSize mxGetNzmax_700_proxy(const mxArray *a0);
#endif

#if defined(MX_COMPAT_32)
EXTERN_C
void mxSetNzmax_700_proxy(mxArray *a0, mwSize a1);
#endif

#if defined(MX_COMPAT_32)
EXTERN_C
mxArray * mxGetFieldByNumber_700_proxy(const mxArray *a0, mwIndex a1, 
    int a2);
#endif

#if defined(MX_COMPAT_32)
EXTERN_C
mxArray * mxGetCell_700_proxy(const mxArray *a0, mwIndex a1);
#endif

#if defined(MX_COMPAT_32)
EXTERN_C
void mxSetM_700_proxy(mxArray *a0, mwSize a1);
#endif

#if defined(MX_COMPAT_32)
EXTERN_C
void mxSetIr_700_proxy(mxArray *a0, mwIndex *a1);
#endif

#if defined(MX_COMPAT_32)
EXTERN_C
void mxSetJc_700_proxy(mxArray *a0, mwIndex *a1);
#endif

#if defined(MX_COMPAT_32)
EXTERN_C
mwIndex mxCalcSingleSubscript_700_proxy(const mxArray *a0, mwSize a1, 
    const mwIndex *a2);
#endif

#if defined(MX_COMPAT_32)
EXTERN_C
void mxSetCell_700_proxy(mxArray *a0, mwIndex a1, mxArray *a2);
#endif

#if defined(MX_COMPAT_32)
EXTERN_C
void mxSetFieldByNumber_700_proxy(mxArray *a0, mwIndex a1, int a2, 
    mxArray *a3);
#endif

#if defined(MX_COMPAT_32)
EXTERN_C
mxArray * mxGetField_700_proxy(const mxArray *a0, mwIndex a1, 
    const char *a2);
#endif

#if defined(MX_COMPAT_32)
EXTERN_C
void mxSetField_700_proxy(mxArray *a0, mwIndex a1, const char *a2, 
    mxArray *a3);
#endif

#if defined(MX_COMPAT_32)
EXTERN_C
mxArray * mxCreateNumericMatrix_700_proxy(mwSize a0, mwSize a1, 
    mxClassID a2, mxComplexity a3);
#endif

#if defined(MX_COMPAT_32)
EXTERN_C
void mxSetN_700_proxy(mxArray *a0, mwSize a1);
#endif

#if defined(MX_COMPAT_32)
EXTERN_C
int mxSetDimensions_700_proxy(mxArray *a0, const mwSize *a1, mwSize a2);
#endif

#if defined(MX_COMPAT_32)
EXTERN_C
mxArray * mxCreateNumericArray_700_proxy(mwSize a0, const mwSize *a1, 
    mxClassID a2, mxComplexity a3);
#endif

#if defined(MX_COMPAT_32)
EXTERN_C
mxArray * mxCreateCharArray_700_proxy(mwSize a0, const mwSize *a1);
#endif

#if defined(MX_COMPAT_32)
EXTERN_C
mxArray * mxCreateDoubleMatrix_700_proxy(mwSize a0, mwSize a1, 
    mxComplexity a2);
#endif

#if defined(MX_COMPAT_32)
EXTERN_C
mxArray * mxCreateLogicalArray_700_proxy(mwSize a0, const mwSize *a1);
#endif

#if defined(MX_COMPAT_32)
EXTERN_C
mxArray * mxCreateLogicalMatrix_700_proxy(mwSize a0, mwSize a1);
#endif

#if defined(MX_COMPAT_32)
EXTERN_C
mxArray * mxCreateSparse_700_proxy(mwSize a0, mwSize a1, mwSize a2, 
    mxComplexity a3);
#endif

#if defined(MX_COMPAT_32)
EXTERN_C
mxArray * mxCreateSparseLogicalMatrix_700_proxy(mwSize a0, mwSize a1, 
    mwSize a2);
#endif

#if defined(MX_COMPAT_32)
EXTERN_C
void mxGetNChars_700_proxy(const mxArray *a0, char *a1, mwSize a2);
#endif

#if defined(MX_COMPAT_32)
EXTERN_C
int mxGetString_700_proxy(const mxArray *a0, char *a1, mwSize a2);
#endif

#if defined(MX_COMPAT_32)
EXTERN_C
mxArray * mxCreateStringFromNChars_700_proxy(const char *a0, mwSize a1);
#endif

#if defined(MX_COMPAT_32)
EXTERN_C
mxArray * mxCreateCharMatrixFromStrings_700_proxy(mwSize a0, 
    const char **a1);
#endif

#if defined(MX_COMPAT_32)
EXTERN_C
mxArray * mxCreateCellMatrix_700_proxy(mwSize a0, mwSize a1);
#endif

#if defined(MX_COMPAT_32)
EXTERN_C
mxArray * mxCreateCellArray_700_proxy(mwSize a0, const mwSize *a1);
#endif

#if defined(MX_COMPAT_32)
EXTERN_C
mxArray * mxCreateStructMatrix_700_proxy(mwSize a0, mwSize a1, int a2, 
    const char **a3);
#endif

#if defined(MX_COMPAT_32)
EXTERN_C
mxArray * mxCreateStructArray_700_proxy(mwSize a0, const mwSize *a1, 
    int a2, const char **a3);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
void * mxMalloc_proxy(size_t a0);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
void * mxCalloc_proxy(size_t a0, size_t a1);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
void mxFree_proxy(void *a0);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
void * mxRealloc_proxy(void *a0, size_t a1);
#endif

#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
EXTERN_C
mwSize mxGetNumberOfDimensions_730_proxy(const mxArray *a0);
#endif

#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
EXTERN_C
const mwSize * mxGetDimensions_730_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
size_t mxGetM_proxy(const mxArray *a0);
#endif

#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
EXTERN_C
mwIndex * mxGetIr_730_proxy(const mxArray *a0);
#endif

#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
EXTERN_C
mwIndex * mxGetJc_730_proxy(const mxArray *a0);
#endif

#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
EXTERN_C
mwSize mxGetNzmax_730_proxy(const mxArray *a0);
#endif

#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
EXTERN_C
void mxSetNzmax_730_proxy(mxArray *a0, mwSize a1);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
const char * mxGetFieldNameByNumber_proxy(const mxArray *a0, int a1);
#endif

#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
EXTERN_C
mxArray * mxGetFieldByNumber_730_proxy(const mxArray *a0, mwIndex a1, 
    int a2);
#endif

#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
EXTERN_C
mxArray * mxGetCell_730_proxy(const mxArray *a0, mwIndex a1);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
mxClassID mxGetClassID_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
void * mxGetData_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
void mxSetData_proxy(mxArray *a0, void *a1);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
bool mxIsNumeric_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
bool mxIsCell_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
bool mxIsLogical_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
bool mxIsScalar_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
bool mxIsChar_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
bool mxIsStruct_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
bool mxIsOpaque_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
bool mxIsFunctionHandle_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
bool mxIsObject_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
void * mxGetImagData_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
void mxSetImagData_proxy(mxArray *a0, void *a1);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
bool mxIsComplex_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
bool mxIsSparse_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
bool mxIsDouble_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
bool mxIsSingle_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
bool mxIsInt8_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
bool mxIsUint8_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
bool mxIsInt16_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
bool mxIsUint16_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
bool mxIsInt32_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
bool mxIsUint32_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
bool mxIsInt64_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
bool mxIsUint64_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
size_t mxGetNumberOfElements_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
double * mxGetPi_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
void mxSetPi_proxy(mxArray *a0, double *a1);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
mxChar * mxGetChars_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
int mxGetUserBits_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
void mxSetUserBits_proxy(mxArray *a0, int a1);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
double mxGetScalar_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
bool mxIsFromGlobalWS_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
void mxSetFromGlobalWS_proxy(mxArray *a0, bool a1);
#endif

#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
EXTERN_C
void mxSetM_730_proxy(mxArray *a0, mwSize a1);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
size_t mxGetN_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
bool mxIsEmpty_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
int mxGetFieldNumber_proxy(const mxArray *a0, const char *a1);
#endif

#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
EXTERN_C
void mxSetIr_730_proxy(mxArray *a0, mwIndex *a1);
#endif

#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
EXTERN_C
void mxSetJc_730_proxy(mxArray *a0, mwIndex *a1);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
double * mxGetPr_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
void mxSetPr_proxy(mxArray *a0, double *a1);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
size_t mxGetElementSize_proxy(const mxArray *a0);
#endif

#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
EXTERN_C
mwIndex mxCalcSingleSubscript_730_proxy(const mxArray *a0, mwSize a1, 
    const mwIndex *a2);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
int mxGetNumberOfFields_proxy(const mxArray *a0);
#endif

#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
EXTERN_C
void mxSetCell_730_proxy(mxArray *a0, mwIndex a1, mxArray *a2);
#endif

#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
EXTERN_C
void mxSetFieldByNumber_730_proxy(mxArray *a0, mwIndex a1, int a2, 
    mxArray *a3);
#endif

#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
EXTERN_C
mxArray * mxGetField_730_proxy(const mxArray *a0, mwIndex a1, 
    const char *a2);
#endif

#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
EXTERN_C
void mxSetField_730_proxy(mxArray *a0, mwIndex a1, const char *a2, 
    mxArray *a3);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
bool mxIsClass_proxy(const mxArray *a0, const char *a1);
#endif

#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
EXTERN_C
mxArray * mxCreateNumericMatrix_730_proxy(mwSize a0, mwSize a1, 
    mxClassID a2, mxComplexity a3);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
mxArray * mxCreateUninitNumericMatrix_proxy(size_t a0, size_t a1, 
    mxClassID a2, mxComplexity a3);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
mxArray * mxCreateUninitNumericArray_proxy(size_t a0, size_t *a1, 
    mxClassID a2, mxComplexity a3);
#endif

#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
EXTERN_C
void mxSetN_730_proxy(mxArray *a0, mwSize a1);
#endif

#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
EXTERN_C
int mxSetDimensions_730_proxy(mxArray *a0, const mwSize *a1, mwSize a2);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
void mxDestroyArray_proxy(mxArray *a0);
#endif

#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
EXTERN_C
mxArray * mxCreateNumericArray_730_proxy(mwSize a0, const mwSize *a1, 
    mxClassID a2, mxComplexity a3);
#endif

#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
EXTERN_C
mxArray * mxCreateCharArray_730_proxy(mwSize a0, const mwSize *a1);
#endif

#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
EXTERN_C
mxArray * mxCreateDoubleMatrix_730_proxy(mwSize a0, mwSize a1, 
    mxComplexity a2);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
mxLogical * mxGetLogicals_proxy(const mxArray *a0);
#endif

#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
EXTERN_C
mxArray * mxCreateLogicalArray_730_proxy(mwSize a0, const mwSize *a1);
#endif

#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
EXTERN_C
mxArray * mxCreateLogicalMatrix_730_proxy(mwSize a0, mwSize a1);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
mxArray * mxCreateLogicalScalar_proxy(bool a0);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
bool mxIsLogicalScalar_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
bool mxIsLogicalScalarTrue_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
mxArray * mxCreateDoubleScalar_proxy(double a0);
#endif

#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
EXTERN_C
mxArray * mxCreateSparse_730_proxy(mwSize a0, mwSize a1, mwSize a2, 
    mxComplexity a3);
#endif

#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
EXTERN_C
mxArray * mxCreateSparseLogicalMatrix_730_proxy(mwSize a0, mwSize a1, 
    mwSize a2);
#endif

#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
EXTERN_C
void mxGetNChars_730_proxy(const mxArray *a0, char *a1, mwSize a2);
#endif

#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
EXTERN_C
int mxGetString_730_proxy(const mxArray *a0, char *a1, mwSize a2);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
char * mxArrayToString_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
char * mxArrayToUTF8String_proxy(const mxArray *a0);
#endif

#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
EXTERN_C
mxArray * mxCreateStringFromNChars_730_proxy(const char *a0, mwSize a1);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
mxArray * mxCreateString_proxy(const char *a0);
#endif

#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
EXTERN_C
mxArray * mxCreateCharMatrixFromStrings_730_proxy(mwSize a0, 
    const char **a1);
#endif

#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
EXTERN_C
mxArray * mxCreateCellMatrix_730_proxy(mwSize a0, mwSize a1);
#endif

#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
EXTERN_C
mxArray * mxCreateCellArray_730_proxy(mwSize a0, const mwSize *a1);
#endif

#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
EXTERN_C
mxArray * mxCreateStructMatrix_730_proxy(mwSize a0, mwSize a1, int a2, 
    const char **a3);
#endif

#if !defined(MX_COMPAT_32) && TARGET_API_VERSION < 800
EXTERN_C
mxArray * mxCreateStructArray_730_proxy(mwSize a0, const mwSize *a1, 
    int a2, const char **a3);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
mxArray * mxDuplicateArray_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
int mxSetClassName_proxy(mxArray *a0, const char *a1);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
int mxAddField_proxy(mxArray *a0, const char *a1);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
void mxRemoveField_proxy(mxArray *a0, int a1);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
double mxGetEps_proxy();
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
double mxGetInf_proxy();
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
double mxGetNaN_proxy();
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
bool mxIsFinite_proxy(double a0);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
bool mxIsInf_proxy(double a0);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
bool mxIsNaN_proxy(double a0);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
mxArray * mxCreateSharedDataCopy_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
mxArray * mxCreateUninitDoubleMatrix_proxy(int a0, size_t a1, size_t a2);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
mxArray * mxFastZeros_proxy(int a0, int a1, int a2);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
mxArray * mxUnreference_proxy(mxArray *a0);
#endif

#if TARGET_API_VERSION < 800
EXTERN_C
int mxUnshareArray_proxy(mxArray *a0, int a1);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
void * mxMalloc_800_proxy(size_t a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
void * mxCalloc_800_proxy(size_t a0, size_t a1);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
void mxFree_800_proxy(void *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
void * mxRealloc_800_proxy(void *a0, size_t a1);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
mwSize mxGetNumberOfDimensions_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
const mwSize * mxGetDimensions_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
size_t mxGetM_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
mwIndex * mxGetIr_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
mwIndex * mxGetJc_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
mwSize mxGetNzmax_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
void mxSetNzmax_800_proxy(mxArray *a0, mwSize a1);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
const char * mxGetFieldNameByNumber_800_proxy(const mxArray *a0, int a1);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
mxArray * mxGetFieldByNumber_800_proxy(const mxArray *a0, mwIndex a1, 
    int a2);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
mxArray * mxGetCell_800_proxy(const mxArray *a0, mwIndex a1);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
mxClassID mxGetClassID_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
void * mxGetData_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
void mxSetData_800_proxy(mxArray *a0, void *a1);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
bool mxIsNumeric_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
bool mxIsCell_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
bool mxIsLogical_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
bool mxIsScalar_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
bool mxIsChar_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
bool mxIsStruct_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
bool mxIsOpaque_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
bool mxIsFunctionHandle_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
bool mxIsObject_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
bool mxIsComplex_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
bool mxIsSparse_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
bool mxIsDouble_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
bool mxIsSingle_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
bool mxIsInt8_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
bool mxIsUint8_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
bool mxIsInt16_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
bool mxIsUint16_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
bool mxIsInt32_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
bool mxIsUint32_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
bool mxIsInt64_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
bool mxIsUint64_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
size_t mxGetNumberOfElements_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
mxChar * mxGetChars_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
int mxGetUserBits_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
void mxSetUserBits_800_proxy(mxArray *a0, int a1);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
double mxGetScalar_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
bool mxIsFromGlobalWS_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
void mxSetFromGlobalWS_800_proxy(mxArray *a0, bool a1);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
void mxSetM_800_proxy(mxArray *a0, mwSize a1);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
size_t mxGetN_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
bool mxIsEmpty_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
int mxGetFieldNumber_800_proxy(const mxArray *a0, const char *a1);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
void mxSetIr_800_proxy(mxArray *a0, mwIndex *a1);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
void mxSetJc_800_proxy(mxArray *a0, mwIndex *a1);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
double * mxGetPr_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
void mxSetPr_800_proxy(mxArray *a0, double *a1);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
size_t mxGetElementSize_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
mwIndex mxCalcSingleSubscript_800_proxy(const mxArray *a0, mwSize a1, 
    const mwIndex *a2);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
int mxGetNumberOfFields_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
void mxSetCell_800_proxy(mxArray *a0, mwIndex a1, mxArray *a2);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
void mxSetFieldByNumber_800_proxy(mxArray *a0, mwIndex a1, int a2, 
    mxArray *a3);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
mxArray * mxGetField_800_proxy(const mxArray *a0, mwIndex a1, 
    const char *a2);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
void mxSetField_800_proxy(mxArray *a0, mwIndex a1, const char *a2, 
    mxArray *a3);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
bool mxIsClass_800_proxy(const mxArray *a0, const char *a1);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
mxArray * mxCreateNumericMatrix_800_proxy(mwSize a0, mwSize a1, 
    mxClassID a2, mxComplexity a3);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
mxArray * mxCreateUninitNumericMatrix_800_proxy(size_t a0, size_t a1, 
    mxClassID a2, mxComplexity a3);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
mxArray * mxCreateUninitNumericArray_800_proxy(size_t a0, size_t *a1, 
    mxClassID a2, mxComplexity a3);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
void mxSetN_800_proxy(mxArray *a0, mwSize a1);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
int mxSetDimensions_800_proxy(mxArray *a0, const mwSize *a1, mwSize a2);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
void mxDestroyArray_800_proxy(mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
mxArray * mxCreateNumericArray_800_proxy(mwSize a0, const mwSize *a1, 
    mxClassID a2, mxComplexity a3);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
mxArray * mxCreateCharArray_800_proxy(mwSize a0, const mwSize *a1);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
mxArray * mxCreateDoubleMatrix_800_proxy(mwSize a0, mwSize a1, 
    mxComplexity a2);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
mxLogical * mxGetLogicals_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
mxArray * mxCreateLogicalArray_800_proxy(mwSize a0, const mwSize *a1);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
mxArray * mxCreateLogicalMatrix_800_proxy(mwSize a0, mwSize a1);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
mxArray * mxCreateLogicalScalar_800_proxy(bool a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
bool mxIsLogicalScalar_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
bool mxIsLogicalScalarTrue_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
mxArray * mxCreateDoubleScalar_800_proxy(double a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
mxArray * mxCreateSparse_800_proxy(mwSize a0, mwSize a1, mwSize a2, 
    mxComplexity a3);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
mxArray * mxCreateSparseLogicalMatrix_800_proxy(mwSize a0, mwSize a1, 
    mwSize a2);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
void mxGetNChars_800_proxy(const mxArray *a0, char *a1, mwSize a2);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
int mxGetString_800_proxy(const mxArray *a0, char *a1, mwSize a2);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
char * mxArrayToString_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
char * mxArrayToUTF8String_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
mxArray * mxCreateStringFromNChars_800_proxy(const char *a0, mwSize a1);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
mxArray * mxCreateString_800_proxy(const char *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
mxArray * mxCreateCharMatrixFromStrings_800_proxy(mwSize a0, 
    const char **a1);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
mxArray * mxCreateCellMatrix_800_proxy(mwSize a0, mwSize a1);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
mxArray * mxCreateCellArray_800_proxy(mwSize a0, const mwSize *a1);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
mxArray * mxCreateStructMatrix_800_proxy(mwSize a0, mwSize a1, int a2, 
    const char **a3);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
mxArray * mxCreateStructArray_800_proxy(mwSize a0, const mwSize *a1, 
    int a2, const char **a3);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
mxArray * mxDuplicateArray_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
int mxSetClassName_800_proxy(mxArray *a0, const char *a1);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
int mxAddField_800_proxy(mxArray *a0, const char *a1);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
void mxRemoveField_800_proxy(mxArray *a0, int a1);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
double mxGetEps_800_proxy();
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
double mxGetInf_800_proxy();
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
double mxGetNaN_800_proxy();
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
bool mxIsFinite_800_proxy(double a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
bool mxIsInf_800_proxy(double a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
bool mxIsNaN_800_proxy(double a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
mxDouble * mxGetDoubles_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
int mxSetDoubles_800_proxy(mxArray *a0, mxDouble *a1);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
mxComplexDouble * mxGetComplexDoubles_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
int mxSetComplexDoubles_800_proxy(mxArray *a0, mxComplexDouble *a1);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
mxSingle * mxGetSingles_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
int mxSetSingles_800_proxy(mxArray *a0, mxSingle *a1);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
mxComplexSingle * mxGetComplexSingles_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
int mxSetComplexSingles_800_proxy(mxArray *a0, mxComplexSingle *a1);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
mxInt8 * mxGetInt8s_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
int mxSetInt8s_800_proxy(mxArray *a0, mxInt8 *a1);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
mxComplexInt8 * mxGetComplexInt8s_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
int mxSetComplexInt8s_800_proxy(mxArray *a0, mxComplexInt8 *a1);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
mxUint8 * mxGetUint8s_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
int mxSetUint8s_800_proxy(mxArray *a0, mxUint8 *a1);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
mxComplexUint8 * mxGetComplexUint8s_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
int mxSetComplexUint8s_800_proxy(mxArray *a0, mxComplexUint8 *a1);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
mxInt16 * mxGetInt16s_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
int mxSetInt16s_800_proxy(mxArray *a0, mxInt16 *a1);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
mxComplexInt16 * mxGetComplexInt16s_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
int mxSetComplexInt16s_800_proxy(mxArray *a0, mxComplexInt16 *a1);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
mxUint16 * mxGetUint16s_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
int mxSetUint16s_800_proxy(mxArray *a0, mxUint16 *a1);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
mxComplexUint16 * mxGetComplexUint16s_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
int mxSetComplexUint16s_800_proxy(mxArray *a0, mxComplexUint16 *a1);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
mxInt32 * mxGetInt32s_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
int mxSetInt32s_800_proxy(mxArray *a0, mxInt32 *a1);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
mxComplexInt32 * mxGetComplexInt32s_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
int mxSetComplexInt32s_800_proxy(mxArray *a0, mxComplexInt32 *a1);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
mxUint32 * mxGetUint32s_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
int mxSetUint32s_800_proxy(mxArray *a0, mxUint32 *a1);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
mxComplexUint32 * mxGetComplexUint32s_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
int mxSetComplexUint32s_800_proxy(mxArray *a0, mxComplexUint32 *a1);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
mxInt64 * mxGetInt64s_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
int mxSetInt64s_800_proxy(mxArray *a0, mxInt64 *a1);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
mxComplexInt64 * mxGetComplexInt64s_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
int mxSetComplexInt64s_800_proxy(mxArray *a0, mxComplexInt64 *a1);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
mxUint64 * mxGetUint64s_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
int mxSetUint64s_800_proxy(mxArray *a0, mxUint64 *a1);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
mxComplexUint64 * mxGetComplexUint64s_800_proxy(const mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
int mxSetComplexUint64s_800_proxy(mxArray *a0, mxComplexUint64 *a1);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
int mxMakeArrayReal_800_proxy(mxArray *a0);
#endif

#if TARGET_API_VERSION >= 800
EXTERN_C
int mxMakeArrayComplex_800_proxy(mxArray *a0);
#endif



#ifdef __cplusplus
extern "C"
{
#endif

/* Standard proxy prolog. Undefine any preprocessor definitions associated
 * with the functions we are proxying, so that we can define a new one. We
 * map the user-called name to the new, internal name.
 */
#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#ifdef mxGetClassName
#undef mxGetClassName
#endif
#define mxGetClassName mxGetClassNameDeployed
#endif

#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#ifdef mxGetProperty
#undef mxGetProperty
#endif
#define mxGetProperty mxGetPropertyDeployed
#endif

#if !(defined(__APPLE__) && (defined(__arm__) || defined(__arm64__)))
#ifdef mxSetProperty
#undef mxSetProperty
#endif
#define mxSetProperty mxSetPropertyDeployed
#endif

/* Use EXTERN_C on these declarations because they are not seen when building
 * the proxy layer. User client code includes the publish header. Therefore
 * the symbols should be declared appropriately for import rather than export.
 */
#if !defined(MW_BUILD_ARCHIVES)
EXTERN_C
const char * mxGetClassNameDeployed(mxArray const * const data);

EXTERN_C
mxArray *mxGetPropertyDeployed(const mxArray *obj, mwIndex index,
                               const char *propName);

EXTERN_C
void mxSetPropertyDeployed(mxArray *obj, mwIndex index,
                           const char *propName, const mxArray *propValue);

#endif

#ifdef __cplusplus
}
#endif


#if defined(__APPLE__) && (defined(__arm__) || defined(__arm64__))
EXTERN_C bool mclmcrInitialize2_proxy(int mode);
EXTERN_C bool mclmcrInitialize_proxy(void);
EXTERN_C bool mclInitializeApplication_proxy(const char **options, size_t count);
#endif

#endif /* mclmcrrt_h */
