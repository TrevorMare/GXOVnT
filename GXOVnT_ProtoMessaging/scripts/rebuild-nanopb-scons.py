Import("env")


def nanopb_callback(source, target, env):
    print("Hello PlatformIO!")
    env.SetDefault(NANOPB = "lib/nanopb")
    env.Tool("nanopb", toolpath = ["lib/nanopb/tests/site_scons/site_tools"])
    env.NanopbProto("proto/simple.proto")


# env.AddCustomTarget(
#     name="nanopb-scons",
#     dependencies=None,
#     actions=[
#         "python --version", nanopb_callback
#     ],
#     title="Generate proto messages",
#     description="Generate proto messages",
#     always_build=True
# )


#env.AddPreAction("buildprog", nanopb_callback)

print("Hello PlatformIO!")
env.SetDefault(NANOPB = "lib/nanopb")
env.Tool("nanopb", toolpath = ["lib/nanopb/tests/site_scons/site_tools"])
env.NanopbProto("proto/simple.proto")