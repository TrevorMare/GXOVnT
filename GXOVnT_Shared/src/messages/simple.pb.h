/* Automatically generated nanopb header */
/* Generated by nanopb-0.4.9-dev */

#ifndef PB_SIMPLE_PB_H_INCLUDED
#define PB_SIMPLE_PB_H_INCLUDED
#include <pb.h>

#if PB_PROTO_HEADER_VERSION != 40
#error Regenerate this file with the current version of nanopb generator.
#endif

/* Struct definitions */
typedef struct _PageHeader {
    bool has_version;
    int32_t version;
    bool has_seq_no;
    int32_t seq_no;
    bool has_erase_count;
    int32_t erase_count;
} PageHeader;

typedef struct _ValueUpdate {
    /* Always present */
    bool has_id;
    int32_t id;
    /* If present, associates id to this name */
    pb_callback_t name;
    /* If none of the following fields is present, this is a tombstone. */
    bool has_uint64_val;
    uint64_t uint64_val;
    bool has_sint64_val;
    int64_t sint64_val;
    bool has_fixed64_val;
    uint64_t fixed64_val;
    bool has_fixed32_val;
    uint32_t fixed32_val;
    pb_callback_t bytes;
} ValueUpdate;


#ifdef __cplusplus
extern "C" {
#endif

/* Initializer values for message structs */
#define PageHeader_init_default                  {false, 0, false, 0, false, 0}
#define ValueUpdate_init_default                 {false, 0, {{NULL}, NULL}, false, 0, false, 0, false, 0, false, 0, {{NULL}, NULL}}
#define PageHeader_init_zero                     {false, 0, false, 0, false, 0}
#define ValueUpdate_init_zero                    {false, 0, {{NULL}, NULL}, false, 0, false, 0, false, 0, false, 0, {{NULL}, NULL}}

/* Field tags (for use in manual encoding/decoding) */
#define PageHeader_version_tag                   1
#define PageHeader_seq_no_tag                    2
#define PageHeader_erase_count_tag               3
#define ValueUpdate_id_tag                       1
#define ValueUpdate_name_tag                     2
#define ValueUpdate_uint64_val_tag               3
#define ValueUpdate_sint64_val_tag               4
#define ValueUpdate_fixed64_val_tag              5
#define ValueUpdate_fixed32_val_tag              6
#define ValueUpdate_bytes_tag                    7

/* Struct field encoding specification for nanopb */
#define PageHeader_FIELDLIST(X, a) \
X(a, STATIC,   OPTIONAL, INT32,    version,           1) \
X(a, STATIC,   OPTIONAL, INT32,    seq_no,            2) \
X(a, STATIC,   OPTIONAL, INT32,    erase_count,       3)
#define PageHeader_CALLBACK NULL
#define PageHeader_DEFAULT NULL

#define ValueUpdate_FIELDLIST(X, a) \
X(a, STATIC,   OPTIONAL, INT32,    id,                1) \
X(a, CALLBACK, OPTIONAL, STRING,   name,              2) \
X(a, STATIC,   OPTIONAL, UINT64,   uint64_val,        3) \
X(a, STATIC,   OPTIONAL, SINT64,   sint64_val,        4) \
X(a, STATIC,   OPTIONAL, FIXED64,  fixed64_val,       5) \
X(a, STATIC,   OPTIONAL, FIXED32,  fixed32_val,       6) \
X(a, CALLBACK, OPTIONAL, BYTES,    bytes,             7)
#define ValueUpdate_CALLBACK pb_default_field_callback
#define ValueUpdate_DEFAULT NULL

extern const pb_msgdesc_t PageHeader_msg;
extern const pb_msgdesc_t ValueUpdate_msg;

/* Defines for backwards compatibility with code written before nanopb-0.4.0 */
#define PageHeader_fields &PageHeader_msg
#define ValueUpdate_fields &ValueUpdate_msg

/* Maximum encoded size of messages (where known) */
/* ValueUpdate_size depends on runtime parameters */
#define PageHeader_size                          33
#define SIMPLE_PB_H_MAX_SIZE                     PageHeader_size

#ifdef __cplusplus
} /* extern "C" */
#endif

#endif
