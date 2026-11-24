# Firmware and recovery safety
Initial inspection supports local SHA-256 calculation, comparison against an independently trusted expected digest, and ZIP entry listing (first 1000 entries, no extraction).
A matching digest is not proof of authenticity, compatibility, or safe installation. No flashing is enabled by this component.
Before any future write operation, require explicit target selection, verified model and variant, OEM-supported image provenance, rollback and bootloader compatibility, power checks, backup planning and explicit confirmation. Unsupported or unknown conditions must block execution.
Recovery and backup support varies by OEM and Android version. Do not promise access to encrypted or protected data.