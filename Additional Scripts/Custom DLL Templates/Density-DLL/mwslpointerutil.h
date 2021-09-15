#if defined(_MSC_VER) || __GNUC__ > 3 || (__GNUC__ == 3 && __GNUC_MINOR__ > 3)
#pragma once
#endif

#ifndef mwslpointerutil_published_h
#define mwslpointerutil_published_h

/* Copyright 2020 The MathWorks, Inc. */

/* Only define EXTERN_C if it hasn't been defined already. This allows
 * individual modules to have more control over managing their exports.
 */
#ifndef EXTERN_C
#define EXTERN_C extern
#endif

#include "tmwtypes.h"

EXTERN_C void suStoreArrayInPointerPool(void* dst, const void* src, int32_t numBytes);
    
#endif /* mwslpointerutil_published_h */
