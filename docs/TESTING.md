# Test checklist

Use a disposable Creative world first. **Back up** the world and dedicated-server configuration before touching a permanent server.

- [ ] Confirm exact mod versions: VS 1.22.7, Farming Plus 5.0.0, Lib Plus 5.0.0, HoD 2.5.8.
- [ ] Remove old compatibility ZIPs from both game and AppData Mods directories.
- [ ] Fully restart the process after the mod DLL is replaced.
- [ ] Confirm `[LTO HoD Water Compat V5] ACTIVE` in `server-main.txt`.
- [ ] Compare gutter behavior with ordinary vanilla water and HoD rainwater; note any discrepancy.
- [ ] Ensure copper sprinkler has the required 9-cell farmland cross beneath its 5x5 footprint (at correct height).
- [ ] Confirm the sprinkler activates and the gutter reports `Water: connected`.
- [ ] Record the **barrel's liters before and after** sprinkler operation.
- [ ] Record the **farmland moisture before and after** operation.
- [ ] Check `server-main.txt` for new errors and verify no regressions in other LTO water-related interactions.
- [ ] Repeat testing on a disposable dedicated server before deploying to the permanent world.
- [ ] If LTO Stamina Plus is installed, perform separate water/drinking and container compatibility tests.

If testing fails, capture `server-main.txt`, mod version list, the gutter/sprinkler tooltip and the barrel contents. Uninstall this mod and restart to revert.
